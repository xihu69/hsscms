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
using System.Text;
using Microsoft.AspNetCore.Hosting;
using System.IO;

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
        private readonly ContentSubscribe contentSubscribe;
        private readonly SubscribeClient subscribeClient;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ContentsController(IFreeSql freeSql, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, ICreateManager createManager,DataInOut dataInOut,ContentSubscribe  contentSubscribe,SubscribeClient subscribeClient, IWebHostEnvironment webHostEnvironment)
        {
            this.freeSql = freeSql;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _createManager = createManager;
            this.dataInOut = dataInOut;
            this.contentSubscribe = contentSubscribe;
            this.subscribeClient = subscribeClient;
            this.webHostEnvironment = webHostEnvironment;
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
        /// <summary> 获取内容更新列表
        /// 
        /// </summary>
        public async Task<string> GetSubscribeUpdate(string siteSign, Dictionary<string, DateTime> lastTicksDic)
        {
            //获取站点订阅列表:分类-最后更新时间
            //按照分类-获取最后时间后的更新：分类-最后更新时间
            //返回更新内容列表，添加订阅执行记录
            var re = await contentSubscribe.GetSubscribeUpdate(siteSign, lastTicksDic);
            var stb = new StringBuilder();
            foreach (var item in re)
            {
                stb.Append($"{item.Item1}|{item.Item2},");
            }
            return stb.ToString();
        }
        /// <summary>获取图书
        /// 
        /// </summary>
        public async Task<ContentSubscribe.ImpInfo[]> GetBooksInfo(string[] bookGids) { 

              return  await  contentSubscribe.GetBooksInfoAsync(bookGids);
        }

        public object UpInfo()
        {
            var request = Request;
            string action = request.Query["action"];
            string hash = request.Query["hash"];
            var fileName = request.Query["fileName"];
            string upPath = webHostEnvironment.WebRootPath + "/upload/";
            var fileExt = Path.GetExtension(fileName).ToLower();
            string path_ok = upPath + hash + fileExt;
            string pathTmp = Name2Tmp(path_ok);

            if (System.IO.File.Exists(path_ok))
                return new { data = new { ok = true } };
            else if (System.IO.File.Exists(pathTmp))
                return new FileInfo(pathTmp).Length.ToString();
            else
                return "0";
        }
        string Name2Tmp(string fileName)
        {
            return fileName + ".tmp";
        }
       public class ResultData<T>: ResultData
        {
            public T Data { get; set; }
          
        }
        public class ResultData {
            public static ResultData Re(string error) {
                if (string.IsNullOrEmpty(error))
                {
                    return new ResultData() { Code=1};
                }
                return new ResultData() { Code= 0, Message=error };
            }
            public static ResultData<T> ReData<T>(T data)
            { 
              return  new ResultData<T>() { Code=0, Data=data};
            }
            public int Code { get; set; }
            public string Message { get; set; }
        }
        public object Upload(List<IFormFile> files)
        {
            var request = Request;
            string action = request.Query["action"];
            string hash = request.Query["hash"];
            var fileName = request.Form["fileName"];
            var fileExt = Path.GetExtension(fileName).ToLower();
            string upPath = webHostEnvironment.WebRootPath + "/upload/";
            int fileCount = files.Count;
            if (fileCount < 1)
                return "数量错误";
            if (string.IsNullOrEmpty(hash))
            {
                //普通上传
                var file = files[0];
                if (string.IsNullOrEmpty(fileName))
                    fileName = file.FileName;
                var path = upPath;
                var fillPath = path + fileName;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var fs = System.IO.File.Create(fillPath);
                file.CopyTo(fs);
                fs.Close();
            }
            else
            {
                //秒传或断点续传
                string path_ok = upPath + hash + fileExt;
                string pathTmp = Name2Tmp(path_ok);
                if (System.IO.File.Exists(path_ok))
                {
                    return "";
                }
                if (fileCount > 0)
                {
                    var file = files[0];
                    using (var fs = System.IO.File.Open(pathTmp, FileMode.Append))
                    {
                        file.CopyTo(fs);
                    }
                }
                bool isOk = request.Query["ok"] == "1";
                if (!isOk)
                    return "1";
                if (System.IO.File.Exists(pathTmp))
                    System.IO.File.Move(pathTmp, path_ok);
            }
            return "";
        }
        #region MyRegion

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
        #endregion
    }
}
