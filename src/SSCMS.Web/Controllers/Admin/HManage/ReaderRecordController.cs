using ELibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.HManage
{
    /// <summary>
    /// 阅读记录，下载记录
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ReaderRecordController : HBaseController<ReaderRecord>
    {
        public ReaderRecordController(IFreeSql freeSql) : base(freeSql)
        {

        }
        public void Add(long userId, long siteId, long contentId, int type)
        {
            var sites = freeSql.Select<Site>().ToList();
            var site = sites.Find(p => p.Id == siteId);
            var content = freeSql.Select<Content>().Where(p => p.Id == contentId).AsTable((p, p2) => site.TableName).First();
            var obj = new ReaderRecord();
            obj.UserId = userId;
            obj.SiteId = siteId;
            obj.ContentId = contentId;
            obj.Type = type;
            obj.SiteGid = site.Guid;
            obj.ContentGid = content.Guid;
            obj.ContentName = content.Title;
            freeSql.Insert(obj).ExecuteAffrowsAsync();
        }
    }
}
