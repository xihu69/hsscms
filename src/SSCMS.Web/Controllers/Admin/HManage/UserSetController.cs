using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ELibrary.Models;
using System.Collections.Generic;
using SSCMS.Repositories;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.HManage
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserSetController : HBaseController<ContentFavorites>
    {
        private readonly IFreeSql freeSql;
        private readonly IContentRepository contentRepository;

        public UserSetController(IFreeSql freeSql, IContentRepository contentRepository) :base(freeSql)
        {
            this.freeSql = freeSql;
            this.contentRepository = contentRepository;
        }

        public List<ContentFavorites> GetFavoritesList(long userId, string siteGid, int page, int pageSize = 20)
        {
            return freeSql.Select<ContentFavorites>().Where(p => p.UserId == userId && siteGid == p.SiteGid).Page(page, pageSize).ToList();
        }
        public virtual void AddFavorites(long userId, string siteGid, string contentGid)
        {
            var sites = freeSql.Select<Site>().ToList();
            var site = sites.Find(p => p.Guid == siteGid);
            var content = freeSql.Select<Content>().Where(p => p.Guid == contentGid).AsTable((p, p2) => site.TableName).First();
            var fav = new ContentFavorites() { UserId = userId, SiteGid = siteGid, SiteId = site.Id, ContentId = content.Id, ContentGid = contentGid };
            freeSql.Insert(fav);
        }
        public void RemoveFavorites(long id)
        {
            freeSql.Delete<ContentFavorites>(id);
        }
        public List<ContentScore> GetScore(long userId, string siteGid, int page, int pageSize = 20)
        {
            return freeSql.Select<ContentScore>().Where(p => p.UserId == userId && siteGid == p.SiteGid).Page(page, pageSize).ToList();
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
        public long SaveScore(long userId, string siteGid, string contentGid, decimal score, long? scoreId = null)
        {
            var sites = freeSql.Select<Site>().ToList();
            var site = sites.Find(p => p.Guid == siteGid);
            var content = freeSql.Select<Content>().Where(p => p.Guid == contentGid).AsTable((p, p2) => site.TableName).First();
            var fav = new ContentScore() { UserId = userId, SiteGid = siteGid, SiteId = site.Id, ContentId = content.Id, ContentGid = contentGid, Score = score };
            if (scoreId != null)
            {
                fav.Id = scoreId.Value;
                freeSql.Update<ContentScore>().SetSource(fav).ExecuteAffrows();
            }
            else
                fav.Id = freeSql.Insert<ContentScore>(fav).ExecuteIdentity();
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
