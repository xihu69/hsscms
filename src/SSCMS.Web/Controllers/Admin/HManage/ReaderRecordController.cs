using Datory;
using ELibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSCMS.Web.Controllers.Admin.HManage
{
    /// <summary>
    /// 阅读记录，下载记录
    /// </summary>
    //[Route("api/[controller]/[action]")]
    [ApiController]
    public class ReaderRecordController : HBaseController<ReaderRecord>
    {
        private readonly ISiteRepository siteRepository;
        private readonly IContentRepository contentRepository;
        private readonly IChannelRepository channelRepository;
        private readonly IAuthManager authManager;

        public ReaderRecordController(ISiteRepository siteRepository, IContentRepository contentRepository, IChannelRepository channelRepository, IFreeSql freeSql, IAuthManager authManager) : base(freeSql)
        {
            this.siteRepository = siteRepository;
            this.contentRepository = contentRepository;
            this.channelRepository = channelRepository;
            this.authManager = authManager;
        }
        [HttpPost]
        public async Task AddAsync([FromBody]ReaderRecord record) //, int type, long siteId, long? contentId, long? userId, string url = null
        {
            //var site = siteRepository.GetAsync((int)record.SiteId);
            var obj = new ReaderRecord();
            if (record.Type == 0&& record.ContentId==null)
            {
                obj.Content = record.Content;
            }
            else
            {
                //var content = freeSql.Select<Content>().Where(p => p.Id ==record.ContentId).AsTable((p, p2) => site.TableName).First();
                var content = await contentRepository.GetAsync((int)record.SiteId, 0, (int)record.ContentId);
                obj.ContentId = record.ContentId;
                obj.ContentGid = content.Guid;
                obj.Content = content.Title;
                //更新点击量         
                //content.Hits++;
               contentRepository.UpdateHitsAsync(content.SiteId, content.ChannelId, content.Id, content.Hits+1);
            }
            obj.UserId = record.UserId;
            obj.SiteId = record.SiteId;
            obj.Type =record.Type;
            var request = Request;
            obj.Ip = PageUtils.GetIpAddress(request);
            obj.UserId=authManager.UserId>0? authManager.UserId: record.UserId;
            freeSql.Insert(obj).ExecuteAffrowsAsync();
        }
        /// <summary>
        /// 获取月份访问统计
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        [HttpGet]
        public List<Tuple<string, int>> SumMonth(long siteId) { 
            var nowYear = new DateTime(DateTime.Now.Year, 1, 1);
            var list=freeSql.Select<ReaderRecord>().Where(p => p.ModifiedTime > nowYear && p.SiteId == siteId).GroupBy(p => new { a = p.ModifiedTime.Value.ToString("yy-MM") }).OrderBy(k => k.Key.a).ToList(p => new { date = p.Key.a, count = p.Count() });
            var sql= freeSql.Select<ReaderRecord>().Where(p => p.ModifiedTime > nowYear && p.SiteId == siteId).GroupBy(p => new { a = p.ModifiedTime.Value.ToString("yy-MM") }).OrderBy(k => k.Key.a).ToSql(p => new { date = p.Key.a, count = p.Count() });
           
            return list.Select(p=>Tuple.Create(p.date.Remove(0,3).TrimStart('0'),p.count)).ToList();
        }
        /// <summary>
        ///图书： 阅读、下载、收藏
        /// </summary>
        /// <returns></returns>
        public  Tuple<long, long, long> bookInfoData(long siteId,long contentId) {
            var a= freeSql.Select<ReaderRecord>().Where(p => p.SiteId == siteId&&p.ContentId==contentId&&p.Type==1).Count();
            var b = freeSql.Select<ReaderRecord>().Where(p => p.SiteId == siteId && p.ContentId == contentId && p.Type == 2).Count();
            var c= freeSql.Select<ContentFavorites>().Where(p=>p.SiteId==siteId&&p.ContentId==contentId).Count();
            return Tuple.Create(a,b,c);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>用户数、月访问量、月活跃用户数、总书籍</returns>
        [HttpGet]
        public async Task<Tuple<long, long, long, long>> InfoCount(int siteId,bool isAll=false)
        {
            var userCount = freeSql.Select<User>().AsTable((p1, p2) => $"{Configuration.ESets.CMSDbPrefix}_User").Where(p => p.SiteId == siteId).Count();
            var m30=DateTime.Now.AddMonths(-1);
            var re = freeSql.Select<ReaderRecord>().Where(p => p.SiteId == siteId).WhereIf(!isAll,p=>p.ModifiedTime> m30).Count();
            var hy = freeSql.Select<ReaderRecord>().Where(p => p.SiteId == siteId && (p.Type == 1 || p.Type == 2)).WhereIf(!isAll, p => p.ModifiedTime > m30).Count();

            var cid = await channelRepository.GetChannelIdByIndexNameAsync(siteId, "书籍资源");
            var query = Q.Where(nameof(Models.Content.SiteId), siteId);
            var channelIds = await channelRepository.GetChannelIdsAsync(siteId, cid, Enums.ScopeType.All);
            query.WhereIn(nameof(Models.Content.ChannelId), channelIds);
            var site = await siteRepository.GetAsync(siteId);
            var bookCount =await contentRepository.GetCountAsync(site.TableName, query);

            return Tuple.Create(userCount, re, hy, (long)bookCount);
        }

    }
}
