using ELibrary.Models;
using ELibrary.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using SSCMS.Utils;
using System.Threading.Tasks;
using SSCMS.Services;
using ELibrary.Service;
namespace SSCMS.Web.Controllers.Admin.HManage
{
    //[Route("api/[controller]")]
    [ApiController]
    public class ContentsController : HBaseController
    {
        private readonly IFreeSql freeSql;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly ICreateManager _createManager;
        private readonly DataInOut dataInOut;

        public ContentsController(IFreeSql freeSql, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, ICreateManager createManager,DataInOut dataInOut)
        {
            this.freeSql = freeSql;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _createManager = createManager;
            this.dataInOut = dataInOut;
        }
        /// <summary>导出图书
        /// 
        /// </summary>
        /// <param name="dto">{siteId:123,time:'',ChannelIds:[],bookIds:[]}</param>
        [HttpPost]
        public async void ExportBooks(DataInOut.ExportIdsIn dto)
        {
            dataInOut.ExportBooksAsync(dto);
            return;
            var tk = new TaskRecord();
            tk.SiteId = dto.SiteId;
            tk.InputData = JsonSerializer.Serialize(dto);
            tk.Type = 1;
            tk.Title = "导出任务";
            var re = freeSql.Insert(tk).ExecuteAffrows();
        }
      
        /// <summary> 导入
        /// 
        /// </summary>
        public async void ImportBooks(int siteId,string filePath) {
          dataInOut.ImportBooks(siteId, filePath);
        }
       
        /// <summary> 设置订阅列表
        /// 
        /// </summary>
        public void SetSubscribeList(BookSubscribe info) {
            freeSql.Insert(info).ExecuteAffrows();
        }
        /// <summary> 获取更新列表
        /// 
        /// </summary>
        public void GetSubscribeUpdate(string siteSign,DateTime lastTicks) { 
            //获取站点订阅列表
            //获取最后时间后的更新
             //返回更新内容列表，添加订阅执行记录
        }
        /// <summary>获取图书
        /// 
        /// </summary>
        public void GetBooksInfo(string[] bookIds) { 
        
        }

        public class ExportBooksReq:IDto
        {
            public int SiteId { get; set; }
            public string ChannelContentIds { get; set; }
            public int[] Channels { get; set; }
            public string ExportType { get; set; }
            //public bool IsAllCheckedLevel { get; set; }
            //public List<int> CheckedLevelKeys { get; set; }
            //public bool IsAllDate { get; set; }
            public DateTime StartDate { get; set; }
            //public DateTime EndDate { get; set; }
            //public bool IsAllColumns { get; set; }
            //public List<string> ColumnNames;
        }
        public class ebooks : IDto 
        {
            //【书籍信息，分类名称，分类id，来源站点唯一标识，类型(目录、内容、单列内容、单列目录)】
            public string ChannelsNamePath { get; set; }
            public string BookName { get; set; }
            public string InfoType { get; set; }

        }
    }
}
