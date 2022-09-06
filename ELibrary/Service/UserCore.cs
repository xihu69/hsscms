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

    //获取用户，及用户站点
    //查询推荐书籍
    //阅读量统计
    //阅读记录查询
    public class UserCore
    {
        private readonly IAuthManager authManager;
        private readonly IUserRepository userRepository;
        private readonly IContentRepository contentRepository;
        private readonly ISiteRepository siteRepository;
        private readonly IFreeSql freeSql;

        public UserCore(IAuthManager authManager, IUserRepository userRepository,IContentRepository contentRepository,ISiteRepository siteRepository,IFreeSql freeSql) {
            this.authManager = authManager;
            this.userRepository = userRepository;
            this.contentRepository = contentRepository;
            this.siteRepository = siteRepository;
            this.freeSql = freeSql;
        }

        public async Task<Tuple<User, Site>> GetNowUserInfoAsync() {
            var userId = authManager.UserId;
            var user = await userRepository.GetByUserIdAsync(userId);
            var site = await siteRepository.GetAsync(user.SiteId);
            return Tuple.Create(user, site);
        }
        public List<Content> Recommend(User user, Site site)
        {
            if(user==null||site==null)
                return new List<Content>();
            var count = (int)freeSql.Select<Content>().AsTable((p1, p2) => site.TableName).Where(p => p.SiteId == user.SiteId && p.Recommend == true).Count();
            var begin = 1;
            if (count > 3)
                begin = DateTime.Now.Millisecond % count;
            if (begin + 2 > count)
            {
                begin = count - 3;
            }
            var recList = freeSql.Select<Content>().AsTable((p1, p2) => site.TableName).Where(p => p.SiteId == user.SiteId && p.Recommend == true).Skip(begin).Take(3).ToList();
            return recList;
        }
        /// <summary>
        /// //阅读，收藏，发布
        /// </summary>
        /// <param name="user"></param>
        /// <param name="site"></param>
        /// <returns>//阅读，收藏，发布</returns>
        public async Task<Tuple<long, long, long>> ReadInfoAsync(User user, Site site) {
            if (user == null || site == null)
                return Tuple.Create(0L, 0L, 0L);
            var userId = user.Id;
            var siteId = site.Id;
            var time=DateTime.Now.AddYears(-1);
         var rr=   freeSql.Select<ReaderRecord>().Where(p => p.UserId == userId && p.SiteId == siteId && p.Type == 1&&p.ModifiedTime>time).Count();
        var fav=    freeSql.Select<ContentFavorites>().Where(p => p.UserId == userId && p.SiteId == siteId && p.ModifiedTime > time).Count();
           // var site = await siteRepository.GetAsync(userId);
          var pus=  freeSql.Select<Content>().AsTable((p1, p2) => site.TableName).Where(p => p.SiteId == siteId && p.UserId == userId&& p.AddDate > time).Count();
            return Tuple.Create(rr, fav, pus);
        }
        public List<Content> Record(User user, Site site)
        {
            if (user == null || site == null)
                return new List<Content>();
            var userId = user.Id;
            var siteId = site.Id;
            var recordList = freeSql.Select<ReaderRecord>().Where(p => p.UserId == userId && p.SiteId == siteId && p.Type == 0).OrderByDescending(p => p.ModifiedTime).Take(30).ToList();
            var ids=recordList.Select(p => p.ContentId).Distinct().ToList();
            var recList = freeSql.Select<Content>().AsTable((p1, p2) => site.TableName).Where(p => p.SiteId == user.SiteId &&ids.Contains(p.Id) ).ToList();

            return recList;
        }
    }
}
