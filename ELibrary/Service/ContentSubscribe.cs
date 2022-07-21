using ELibrary.Models;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELibrary.Service
{
    public class ContentSubscribe
    {
        private readonly IFreeSql freeSql;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly ICreateManager _createManager;
        public ContentSubscribe(IFreeSql freeSql, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, ICreateManager createManager)
        {

            this.freeSql = freeSql;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _createManager = createManager;
        }

        /// <summary>
        /// 获取订阅
        /// </summary>
        /// <param name="toSiteName"></param>
        /// <returns></returns>
        public BookSubscribe GetSubscribeList(string toSite)
        {
            return freeSql.Select<BookSubscribe>().Where(p => p.ToStie == toSite).First();
        }
        /// <summary>
        /// 设置订阅列表
        /// </summary>
        /// <param name="info"></param>
        public int SetSubscribeList(BookSubscribe info)
        {
            //todo 应是更新
            var old = freeSql.Select<BookSubscribe>().Where(p => p.ToStie == info.ToStie).First();
            if (old == null)
                return freeSql.Insert(info).ExecuteAffrows();
            else
            {
                old.ChannelGids = info.ChannelGids;
                old.LastRunInfo = $"{DateTime.Now}:设置订阅列表; \n" + old.LastRunInfo.Substring(0, 500);
                return freeSql.Update<BookSubscribe>().SetSource(old).UpdateColumns(x => new { x.ChannelGids, x.LastRunInfo }).ExecuteAffrows();
            }
        }
        /// <summary>
        /// 开关订阅
        /// </summary>
        /// <param name="toSiteName"></param>
        /// <param name="setState"></param>
        /// <returns></returns>
        public int SwitchSubscribe(string toSiteName, int setState)
        {
            var old = freeSql.Select<BookSubscribe>().Where(p => p.ToStie == toSiteName).First();
            old.State = setState;
            return freeSql.Update<BookSubscribe>().SetSource(old).UpdateColumns(x => new { x.State }).ExecuteAffrows();
        }
        /// <summary> 获取图书信息
        /// 
        /// </summary>
        /// <param name="bookGids"></param>
        /// <returns></returns>
        public async Task<ImpInfo[]> GetBooksInfoAsync(string[] bookGids)
        {
            if (bookGids.Length < 1 && bookGids.Length > 1000)
                return null;
            var site = await _siteRepository.GetSiteBySiteNameAsync("main"); //获取订阅站点
            var list = freeSql.Select<Content>().AsTable((_, _) => site.TableName).Where(p => bookGids.Contains(p.Guid)).ToList();
            var channels = await _channelRepository.GetChannelsAsync(site.Id);
            var impInfos = list.Select(p => ToImpInfo(p, channels, site)).ToArray();
            return impInfos;
        }

        

        /// <summary> 获取更新列表,返回内容id和审核通过时间
        /// 
        /// </summary>
      public    async Task<List<Tuple<string, DateTime>>> GetSubscribeUpdate(string siteSign, Dictionary<string, DateTime> lastTicksDic)
        {
            //按分类取更新时间，防止订阅时：添加新的分类订阅，但新分类的历史更新获取不到
            //获取站点订阅列表:分类-最后更新时间
            //按照分类-获取最后时间后的更新：分类-最后更新时间
            //返回更新内容列表，添加订阅执行记录
            var bs = freeSql.Select<BookSubscribe>().Where(p => p.ToStie == siteSign).First();
            var site = await _siteRepository.GetSiteBySiteNameAsync("main"); //获取订阅站点
            var channels = await _channelRepository.GetChannelsAsync(site.Id);
            var gids = bs.ChannelGids.Split(',');
            var re = new List<Tuple<string, DateTime>>();
            foreach (var item in lastTicksDic)
            {
                if (!gids.Any(p => p == item.Key))
                    continue;
                //todo 这里应使用审核通过时间
                var newContents = freeSql.Select<Content>().AsTable((p, p2) => site.TableName).Where(p => p.Checked && p.CreatedDate > item.Value).ToList(p => new { p.Guid, p.CreatedDate });
                var arr = newContents.Select(p => Tuple.Create(p.Guid, p.CreatedDate.Value)).ToArray();
                re.AddRange(arr);
                //foreach (var item2 in newContents)
                //{
                //    stb.Append($"{item2.Guid}|{item2.CreatedDate},");  //gid|审核通过时间
                //}
                //var arr = newContents.Select(p => ToImpInfo(p, channels, site)).ToArray();
            }
            return re;
        }
        public ImpInfo ToImpInfo(Content content, List<Channel> channels, Site site)
        {
            var channel = channels.Find(p => p.Id == content.ChannelId);
            var cChannel = channel;
            List<Channel> path = new List<Channel>();
            while (channel != null)
            {
                path.Add(channel);
                channel = channels.Find(p => p.Id == channel.ParentId);
            }
            path.Reverse();

            var gidPath = string.Join('/', path.Select(p => p.Guid).ToList());
            var namePath = string.Join('/', path.Select(p => p.ChannelName).ToList());
            return new ImpInfo() { GidPath = gidPath, NamePath = namePath, ImpType = 3, Info = content, ParentGuid = cChannel?.Guid, ParentName = cChannel?.ChannelName };
        }


        public class ImpInfo
        {
            public int ImpType { get; set; }
            public string ParentGuid { get; set; }
            public string ParentName { get; set; }

            public string NamePath { get; set; }
            public string GidPath { get; set; }
            public Content Info { get; set; }
        }

    }
}
