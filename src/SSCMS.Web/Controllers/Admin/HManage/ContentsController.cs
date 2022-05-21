using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using System;
using System.Collections.Generic;

namespace SSCMS.Web.Controllers.Admin.HManage
{
    //[Route("api/[controller]")]
    [ApiController]
    public class ContentsController : HBaseController
    {
        private readonly IFreeSql freeSql;

        public ContentsController(IFreeSql freeSql) {
            this.freeSql = freeSql;
        }
        /// <summary>导出图书
        /// 
        /// </summary>
        /// <param name="dto">{siteId:123,time:'',ChannelIds:[],bookIds:[]}</param>
        [HttpPost]
        public void ExportBooks(ExportIdsIn dto)
        {
            //创建导出记录,写入数据库，执行过程修改状态
            //后台执行V
            //查询分类
            //分类内容查询--分页循环写入
            //查询单列内容及分类--分页循环
            //检查内容是否在已有分类中
            //在--忽略
            //不在--添加到分类中，标记单列
            //写入内容 -- 标记单列
            //导出csv结构：【书籍信息，分类名称，分类id，来源站点唯一标识，类型(目录、内容、单列内容、单列目录)】
            //建立后台任务
        }
        /// <summary> 导入
        /// 
        /// </summary>
        public void ImportBooks(string filePath) {
          var re=  freeSql.Ado.Query<dynamic>("select * from siteserver_Log");
            Console.WriteLine(re);
        }
        /// <summary> 设置订阅列表
        /// 
        /// </summary>
        public void SetSubscribeList(string[] books) { 
        }
        /// <summary> 获取更新列表
        /// 
        /// </summary>
        public void GetSubscribeUpdate(string siteSign) { 
        
        }
        /// <summary>获取图书
        /// 
        /// </summary>
        public void GetBooksInfo(string[] bookIds) { 
        
        }

        /// <summary>
        ///  {siteId:123,time:'',ChannelIds:[],bookIds:[]}
        /// </summary>
        public class ExportIdsIn:IDto
        {
            public long SiteId { get; set; }
            public DateTime Time { get; set; }
            public long ChannelIds { get; set; }
            public long[] bookIds { get; set; }
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
