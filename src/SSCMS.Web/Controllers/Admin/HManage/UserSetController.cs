using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ELibrary.Models;
using System.Collections.Generic;
using SSCMS.Repositories;
using SSCMS.Models;
using SSCMS.Services;
using NSwag.Annotations;
using System.Threading.Tasks;

namespace SSCMS.Web.Controllers.Admin.HManage
{
    //[Route("api/[controller]")]
    [ApiController]
    public class UserSetController : HBaseController<ContentFavorites>
    {
        private readonly IFreeSql freeSql;
        private readonly IContentRepository contentRepository;
        private readonly ISiteRepository siteRepository;
        private readonly IAuthManager authManager;

        public UserSetController(IFreeSql freeSql, IContentRepository contentRepository, ISiteRepository siteRepository, IAuthManager authManager) :base(freeSql)
        {
            this.freeSql = freeSql;
            this.contentRepository = contentRepository;
            this.siteRepository = siteRepository;
            this.authManager = authManager;
        }

        [HttpGet]
        public List<ContentFavorites> GetFavoritesList(long userId, long siteId, int page, int pageSize = 20)
        {
            return freeSql.Select<ContentFavorites>().Where(p => p.UserId == userId && siteId == p.SiteId).Page(page, pageSize).ToList();
        }
        [HttpGet]
        public ContentFavorites GetFavorites(long userId, long siteId, long contentId)
        {
            userId = authManager.UserId;
            return freeSql.Select<ContentFavorites>().Where(p => p.UserId == userId && siteId == p.SiteId&&p.ContentId==contentId).First();
        }
        [HttpPost]
        public virtual async Task<long> AddFavorites(long userId, long siteId, long contentId)
        {
            userId = authManager.UserId;
            var old = freeSql.Select<ContentFavorites>().Where(p => p.UserId == userId && p.ContentId == contentId).First();
            if (old != null)
                return old.Id;
            //var sites = freeSql.Select<Site>().ToList();
            //var site = sites.Find(p => p.Id == siteId);
            //var content = freeSql.Select<Content>().Where(p => p.Id == contentId).AsTable((p, p2) => site.TableName).First();

           
            var content=await contentRepository.GetAsync((int)siteId, 0, (int)contentId);
            var fav = new ContentFavorites() { UserId = userId, SiteId = siteId, ContentId = content.Id, ContentGid = content.Guid };
            fav.Content = System.Text.Json.JsonSerializer.Serialize(content); 
            var id = freeSql.Insert(fav).ExecuteIdentity();
            return id;
        }
        [HttpPost]
        public void RemoveFavorites(long siteId, long contentId)
        {
            var userId = authManager.UserId;
            var old = freeSql.Select<ContentFavorites>().Where(p => p.UserId == userId && p.ContentId == contentId).First();
            if (old != null)
                freeSql.Delete<ContentFavorites>(old.Id).ExecuteAffrows();
        }

        /// <summary>
        /// 获取评分
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="siteId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public List<ContentScore> GetScore(long userId, long siteId, long? contentId=null,int page=1, int pageSize = 20)
        {
            return freeSql.Select<ContentScore>().Where(p => p.UserId == userId && siteId == p.SiteId).WhereIf(contentId!=null,p=>p.ContentId==contentId).Page(page, pageSize).ToList();
        }
        [HttpGet]
        public double AvgScore( long siteId, long contentId)
        {
             
           // contentRepository.UpdateAsync
            return freeSql.Select<ContentScore>().Where(p =>siteId == p.SiteId&&contentId==p.ContentId).Avg(p=>p.Score);
        }
        /// <summary>
        /// 保存或更新评分
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="siteGid"></param>
        /// <param name="contentGid"></param>
        /// <param name="score"></param>
        /// <param name="scoreId"></param>
        /// <returns>id</returns>
        [OpenApiOperation("保存或更新评分", "通过scoreId区分")]
        [HttpPost]
        public async Task<long> SaveScore(long userId, long siteId, long contentId, decimal score)
        {
            userId = authManager.UserId;
            //var sites =await siteRepository.GetAsync((int)siteId);// freeSql.Select<Site>().ToList();
            var site = await siteRepository.GetAsync((int)siteId);// sites.Find(p => p.Id == siteId);
            var content = freeSql.Select<Content>().Where(p => p.Id == contentId).AsTable((p, p2) => site.TableName).First();
           var fav=freeSql.Select<ContentScore>().Where(p => p.SiteId == siteId && p.ContentId == contentId && userId == p.UserId).First();
            if(fav==null)
                fav = new ContentScore() { UserId = userId, SiteGid = site.Guid, SiteId = site.Id, ContentId = content.Id, ContentGid = content.Guid, Score = score };
            fav.Score = score;
            if (fav.Id>0)
            {
                freeSql.Update<ContentScore>().SetSource(fav).ExecuteAffrows();
            }
            else
            {
                fav.Id = freeSql.Insert<ContentScore>(fav).ExecuteIdentity();             
            }
            var sc=(decimal)freeSql.Select<ContentScore>().Where(p => siteId == p.SiteId && contentId == p.ContentId).Avg(p => p.Score);
            content.Score = sc;
            freeSql.Update<Content>().AsTable((p) => site.TableName).SetSource(content).ExecuteAffrows();
            return fav.Id;
        }
    }//ReaderRecordBase
    public abstract class FavoritesController<T> : HBaseController<T> where T : ReaderRecordBase, new()
    {
        private readonly IFreeSql freeSql;
        // private readonly IContentRepository contentRepository;

        public FavoritesController(IFreeSql freeSql) : base(freeSql)
        {
            this.freeSql = freeSql;
            //this.contentRepository = contentRepository;
        }
        public List<T> GetList(long userId, string siteGid, int page, int pageSize = 20)
        {
            return freeSql.Select<T>().Where(p => p.UserId == userId && siteGid == p.SiteGid).Page(page, pageSize).ToList();
        }
        public virtual void Add(long userId, string siteGid, string contentGid)
        {
            var sites = freeSql.Select<Site>().ToList();
            var site = sites.Find(p => p.Guid == siteGid);
            var content = freeSql.Select<Content>().Where(p => p.Guid == contentGid).AsTable((p, p2) => site.TableName).First();
            var fav = new T() { UserId = userId, SiteGid = siteGid, SiteId = site.Id, ContentId = content.Id, ContentGid = contentGid };
            freeSql.Insert(fav);
        }
        public void Remove(long id)
        {
            freeSql.Delete<T>(id);
        }

    }
}
