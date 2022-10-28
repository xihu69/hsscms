using ELibrary.Common;
using ELibrary.Models;
using ELibrary.Utils;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELibrary.Service
{
    public class DataInOut
    {
        private readonly IFreeSql freeSql;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly ICreateManager _createManager;
        public DataInOut(IFreeSql freeSql, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, ICreateManager createManager) {

            this.freeSql = freeSql;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _createManager = createManager;
        }

    public   async Task ExportBooksAsync(ExportIdsIn dto) {
            var site = await _siteRepository.GetAsync(dto.SiteId);
            var channels = await _channelRepository.GetChannelsAsync(dto.SiteId);
            //channels = channels.Where(p => p.ChannelName == "书籍资源").ToList();
            var stream = System.IO.File.OpenWrite($"{DateTime.Now.Ticks}_imp.csv");
            var csvw = new CsvWriter(stream);
            var impChannels = channels.Where(p => dto.ChannelIds.Contains(p.Id)).ToList();
            var childrens = dto.ChannelIds.SelectMany(p => getChildrenList(channels, p)).ToList();
            impChannels.AddRange(childrens);
            impChannels = impChannels.DistinctBy(p => p.Id).ToList();
            //var impcArr=impChannels.Select(p=>new IComparable[] { p.Id, p.Guid, 1, p.Guid, site.Id, 1, p.ChannelName, p.ImageUrl, "p.FileUrl", "p.Body" }).ToArray();
            // csvw.WriteAllLines(impcArr);
            foreach (var chl in impChannels)
            {
                var list = freeSql.Select<Content>().WithSql($"select * from {site.TableName}").Where(p => p.ChannelId == chl.Id).ToList();
                var arr = list.Select(p => ContentToCsv(new DataInOut.ExpInfo() { ParentName = chl.ChannelName, Channel=chl,Info = p })).ToArray();//new IComparable[] { p.Id, p.Guid, chl.ChannelName, p.ChannelId, site.Id, 3, p.Title, p.ImageUrl, p.FileUrl, p.Body }
                csvw.WriteAllLines(arr);
            }
            stream.Close();
            /*
               //创建导出记录,写入数据库，执行过程修改状态
            //后台执行V
            //查询分类
            //分类内容查询--分页循环写入
            //查询单列内容及分类--分页循环
            //检查内容是否在已有分类中
            //在--忽略
            //不在--添加到分类中，标记单列
            //写入内容 -- 标记单列
            //导出csv结构：【书籍信息，父级(分类)名称，父级gid，来源站点唯一标识，类型(1目录、2单列目录、3内容、4单列内容、)】
            //建立后台任务
             */
        }
        public async Task<int> ImportBooks(int siteId, string filePath, Channel channelInfo=null)
        {

            var site = await _siteRepository.GetAsync(siteId);
            var channels = await _channelRepository.GetChannelsAsync(siteId);
            var conList = new List<Content>();
            var upCount = 0;
            var nowChannels = new List<Channel>();
            using (var stream = System.IO.File.OpenRead(filePath))
            {
                var encod = GuessEncoding.GetType(stream,"gb2312");
                stream.Position = 0;
                var csvr = new CsvReader(stream, encod);

                var head = csvr.ReadRow();
                var err = checkHead(head);
                if (!string.IsNullOrEmpty(err)) return 0;
                for (var row = csvr.ReadRow(); row != null; row = csvr.ReadRow())
                {
                    var impInfo = ContentByArr(row);
                    var chl = channels.Find(p => p.IndexName == impInfo.ChannelIndex);
                    if (chl == null)
                        continue;
                    nowChannels.Add(chl);
                    impInfo.Info.ChannelId = chl.Id;
                    impInfo.Info.SiteId = siteId;
                    conList.Add(impInfo.Info);
                    if (conList.Count > 100)
                    {
                        var re = await CreateContents(siteId, conList);
                        upCount += re;
                        conList.Clear();
                    }
                }
            }
            if (conList.Count > 0)
            {
                var re = await CreateContents(siteId, conList);
                upCount += re;
                conList.Clear();
            }
            if (upCount > 0)
            {
                foreach (var item in nowChannels)
                {
                    _contentRepository.RemoveListCacheAsync(site, item);
                }
                _createManager.CreateByAllAsync(siteId);
            }


            //建立栏目

            //插入内容

            //更新栏目
            //  await _createManager.CreateContentAsync(request.SiteId, channelInfo.Id, contentId);
            //Datory.Q.CachingRemove(GetListKey(tableName, content.SiteId, content.ChannelId))


            return upCount;
        }

        public async Task<int> CreateContents(int siteId, IList<Content> list)
        {
            var request = list.FirstOrDefault();
            if (request == null)
                return 0;
            //var siteId = request.SiteId;
            var site = await _siteRepository.GetAsync(siteId);
            if (site == null) throw new ArgumentException(nameof(siteId));
            var channels = await _channelRepository.GetChannelsAsync(siteId);
            var listIn = new List<Content>();
            var otherChannel = channels.Find(p => p.ChannelName == "其它分类");
            foreach (var item in list)
            {
                request = item;
                var channelId = request.ChannelId;
                var channel = channels.Find(p => p.Id == channelId);//await _channelRepository.GetAsync(channelId);
                if (channel == null)
                    continue;

                var checkedLevel = site.CheckContentLevel;// request.Checked ? site.CheckContentLevel : request.CheckedLevel;
                var isChecked = checkedLevel >= site.CheckContentLevel;


                var adminId = 0;// _authManager.AdminId;
                var userId = 0;// _authManager.UserId;

                var content = new Content();
                content.LoadDict(request.ToDictionary());
                content.Guid = Guid.NewGuid().ToString(); // request.Guid;
                //content.OrgiVal = request.Guid;
                content.SiteId = siteId;
                content.ChannelId = channelId;
                content.AdminId = adminId;
                content.LastEditAdminId = adminId;
                content.UserId = userId;
                content.SourceId = request.SourceId;
                content.Checked = isChecked;
                content.CheckedLevel = checkedLevel;
                content.LastModifiedDate=DateTime.Now;
                content.AddDate = DateTime.Now;
                content.CreatedDate = DateTime.Now;

                //content.Id = await _contentRepository.InsertAsync(site, channel, content);

                //if (content.Checked)
                //{
                //    await _createManager.CreateContentAsync(siteId, channelId, content.Id);
                //    await _createManager.TriggerContentChangedEventAsync(siteId, channelId);
                //}

                //await _authManager.AddSiteLogAsync(siteId, channelId, content.Id, "添加内容",
                //    $"栏目:{await _channelRepository.GetChannelNameNavigationAsync(siteId, content.ChannelId)},内容标题:{content.Title}");

                listIn.Add(content);
            }
            //string s= freeSql.Insert(listIn).IgnoreColumns(p=>p.Id).AsTable(site.TableName).ToSql();
            var re = freeSql.Insert(listIn).IgnoreColumns(p => p.Id).AsTable(site.TableName).ExecuteAffrows();
            return re;
        }
        public async Task<Result<Channel>> CreateChannel(int siteId, Channel channelInfo)
        {
            
            var site = await _siteRepository.GetAsync(channelInfo.SiteId);
            if (site == null) return Result.To<Channel>("站点无法找到");

            var request = channelInfo;
            if (!string.IsNullOrEmpty(request.IndexName))
            {
                var indexNameList = await _channelRepository.GetIndexNamesAsync(siteId);
                if (indexNameList.Contains(request.IndexName))
                {
                    return Result.To<Channel>("栏目添加失败，栏目索引已存在！");
                }
            }
            if (!string.IsNullOrEmpty(request.FilePath))
            {
                if (!DirectoryUtils.IsDirectoryNameCompliant(request.FilePath))
                {
                    return Result.To<Channel>("栏目页面路径不符合系统要求！");
                }

                if (PathUtils.IsDirectoryPath(request.FilePath))
                {
                    request.FilePath = PageUtils.Combine(request.FilePath, "index.html");
                }

                var filePathList = await _channelRepository.GetAllFilePathBySiteIdAsync(siteId);
                if (filePathList.Contains(request.FilePath))
                {
                    return Result.To<Channel>("栏目添加失败，栏目页面路径已存在！");
                }
            }
            if (!string.IsNullOrEmpty(request.ChannelFilePathRule))
            {
                if (!DirectoryUtils.IsDirectoryNameCompliant(request.ChannelFilePathRule))
                {
                    return Result.To<Channel>("栏目页面命名规则不符合系统要求！");
                }
                if (PathUtils.IsDirectoryPath(request.ChannelFilePathRule))
                {
                    return Result.To<Channel>("栏目页面命名规则必须包含生成文件的后缀！");
                }
            }

            if (!string.IsNullOrEmpty(request.ContentFilePathRule))
            {
                if (!DirectoryUtils.IsDirectoryNameCompliant(request.ContentFilePathRule))
                {
                    return Result.To<Channel>("内容页面命名规则不符合系统要求！");
                }
                if (PathUtils.IsDirectoryPath(request.ContentFilePathRule))
                {
                    return Result.To<Channel>("内容页面命名规则必须包含生成文件的后缀！");
                }
            }
            channelInfo.AddDate = DateTime.Now;
            channelInfo.Id = await _channelRepository.InsertAsync(channelInfo);
            //栏目选择投票样式后，内容

            //await _createManager.CreateChannelAsync(siteId, channelInfo.Id);

            //await _authManager.AddSiteLogAsync(siteId, "添加栏目", $"栏目:{request.ChannelName}");

            return Result.To(channelInfo);
        }
      public  IComparable[] ContentToCsv(ExpInfo p)
        {
            return new IComparable[] { p.Channel.IndexName,  p.Info.Title, p.Info.SubTitle,p.Info.Author, p.Info.ImageUrl, p.Info.FileUrl, p.Info.Summary,p.Info.Guid,p.Info.Body};
        }
       public ImpInfo ContentByArr(string[] p)
        {
            var info = new ImpInfo() { Info=new Content()};

            int i = 0;
            info.ChannelIndex = p[i++];
            info.Info.Title = p[i++];  // 5
            info.Info.SubTitle = p[i++];
            info.Info.Author= p[i++];
            info.Info.ImageUrl = p[i++];
            info.Info.FileUrl = p[i++];
            info.Info.Summary = p[i++];
            info.Info.Guid = p[i++];
            info.Info.Body = p[i++];
            return info;
        }
        public string checkHead(string[] row) {
            var heads = new[] { "分类索引","名称","副标题","作者","图片","文件","内容摘要","来源","内容" };
            if (row.Length != heads.Length)
                return "列数量错误";
            for (int i = 0; i < row.Length; i++) {
                if (row[i] != heads[i])
                    return $"列{i},名称错误";
            }
            return null;
        }
       /// <summary>
       /// 获取所有子节点，平面
       /// </summary>
       /// <param name="list"></param>
       /// <param name="parentId"></param>
       /// <returns></returns>
        public IList<Channel> getChildrenList(IList<Channel> list, int parentId)
        {
            var children1 = list.Where(p => p.ParentId == parentId).ToList();
            var children2 = children1.SelectMany(p => getChildrenList(list, p.Id)).ToArray();
            children1.AddRange(children2);
            return children1;
        }

        
      public  IList<Channel> getChildrensTree(IList<Channel> list, Func<Channel, bool> select = null, Action<Channel, Channel> addChildren = null)
        {
            var roots = new List<Channel>();
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                if (item.ParentId == 0)
                {
                    roots.Add(item);
                    continue;
                }
                for (int y = i + 1; y < list.Count; y++)
                {
                    var item2 = list[y];
                    if (item2.Id == item.ParentId)
                    {
                        if (item2.Children == null)
                            item2.Children = new List<Channel>();
                        item2.Children.Add(item);
                        goto next2;
                    }
                }
                roots.Add(item);
            next2: continue;
            }
            return roots;
        }


        public class ImpInfo
        {
            public string ChannelIndex { get; set; }
            public Content Info { get; set; }
        }


        public class ExpInfo
        {
           // public int ImpType { get; set; }
            public string ParentName { get; set; }

            public Channel Channel { get; set; }
            public Content Info { get; set; }
        }

        /// <summary>
        ///  {siteId:123,time:'',ChannelIds:[],bookIds:[]}
        /// </summary>
        public class ExportIdsIn 
        {
            public int SiteId { get; set; }
            public DateTime Time { get; set; }
            public int[] ChannelIds { get; set; }
            public int[] bookIds { get; set; }
        }
    }
}
