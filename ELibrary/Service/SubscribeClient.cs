using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ELibrary.Models;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
namespace ELibrary.Service
{
    public class SubscribeClient
    {
        private readonly IFreeSql freeSql;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly ICreateManager _createManager;
        public SubscribeClient(IFreeSql freeSql, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, ICreateManager createManager)
        {

            this.freeSql = freeSql;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _createManager = createManager;
        }
        public async void SubscribeUpdate()
        {
            //todo 未完成
            //更新本地订阅列表
            //查询本地更新时间
            //获取主站更新列表
            //保存更新列表
            //添加本地更新时间
            //后台任务执行，获取图书信息
            var listUrl = $"www/GetSubscribeList?toSite=abc";
            var subList = await HttpSendAsync<List<Tuple<string, DateTime>>>(listUrl);
            //var subInfo=  freeSql.Select<BookSubscribe>().Where(p => p.FromSite == fromSite).First();
            var siteName = "main";
            var subUrl = "GetSubscribeUpdate";
            var runTime = DateTime.Now;
            var lastList = SelectSiteLastUpdate("main");
            lastList.ForEach(p => p.QueryBegin = p.QueryEnd);
            var re = await HttpSendAsync<List<Tuple<string, DateTime>>>(subUrl, lastList);
            lastList.ForEach(p => {
                p.Id = 0;
                p.State = 0;
                p.Remark = "";
                p.RunTime = runTime;
                });

            freeSql.Insert(lastList);
            var pull = re.Select(x => new PullRecord() { ContentGid = x.Item1, Newstime = x.Item2, RunRecordId = 0, RunTime = null, State = 0, RunNum = 0 }).ToArray();
            var exCount = freeSql.Insert<PullRecord>(pull).ExecuteAffrows();



        }
        public async Task<T> HttpSendAsync<T>(string url, object data = null,HttpMethod httpMethod = null)
        {
            if (httpMethod == null)
                httpMethod = HttpMethod.Post;  
            HttpClient client = new();
            HttpRequestMessage httpRequest = new HttpRequestMessage(httpMethod, url);
            if (data != null)
            {
                var payload = JsonSerializer.Serialize(data);
                HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");
                httpRequest.Content = content;
            }
            var response = await client.SendAsync(httpRequest);
            var msg = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(msg,null,response.StatusCode);
            }
            if(typeof(T)==typeof(string))
                return (T)(object)msg;
            return JsonSerializer.Deserialize<T>(msg);
        }
        /// <summary>
        /// 获取各个分类最后更新时间
        /// </summary>
        /// <param name="siteName"></param>
        /// <returns></returns>
        List<SubscribeRunRecord> SelectSiteLastUpdate(string siteName)
        {
            //var site = await _siteRepository.GetSiteBySiteNameAsync(siteName); //获取订阅站点
            //var bs = freeSql.Select<BookSubscribe>().Where(p => p.ToStie == siteName).First();
            //var gids = bs.ChannelGids.Split(',');
            //var last = freeSql.Select<Content>().AsTable((p, p2) => site.TableName)
            //       .Where(p => gids.Contains(p.Guid))
            //       .GroupBy(p => p.ChannelId)
            //       .ToList(p => new { channelGid = p.Key, lastTime = p.Max(p.Value.CreatedDate) });
            //return last.Select(p => KeyValuePair.Create(p.channelGid, p.lastTime)).ToArray();

            var lastRun = freeSql.Select<SubscribeRunRecord>().Where(p => p.ToSite == siteName).ToAggregate(p => p.Max(p.Key.RunTime));
            var list = freeSql.Select<SubscribeRunRecord>().Where(p => p.RunTime == lastRun).ToList();

            return list;
        }
    }
}
