using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersController
    {
        [ActivatorUtilitiesConstructor]
        public UsersController(IAuthManager authManager, IPathManager pathManager, IDatabaseManager databaseManager, IUserRepository userRepository, IUserGroupRepository userGroupRepository,IFreeSql freeSql) :this(authManager,pathManager,databaseManager,userRepository,userGroupRepository){
            this.freeSql = freeSql;
        }
        public const string SiteSettingsUsers = "web_settings_users";
        private readonly IFreeSql freeSql;


        public static async Task<bool> chaeckSiteSettingsUsers(IAuthManager authManager, int siteId)
        {
            return siteId < 0 ? false : await authManager.HasSitePermissionsAsync(siteId, SiteSettingsUsers);
        }
        //[controller]/[action]
        [HttpGet, Route(Route+"/[action]")]
        public async Task<ActionResult<GetResults>> SiteUser([FromQuery] SiteGetRequest request)
        {

            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, SiteSettingsUsers))
            {
                return Unauthorized();
            }

            var groups = await _userGroupRepository.GetUserGroupsAsync();

            //var count = await _userRepository.GetCountAsync(request.State, request.GroupId, request.LastActivityDate, request.Keyword);
            ///EL: GetQuery , 变更后无法使用扩展字段
            var sql= freeSql.Select<User>().Where(p => p.SiteId == request.SiteId)
                .WhereIf(!string.IsNullOrWhiteSpace(request.Keyword), p => p.UserName.Contains(request.Keyword) || p.Email.Contains(request.Keyword) || p.Mobile.Contains(request.Keyword) || p.DisplayName.Contains(request.Keyword)).ToSql();

           var users= freeSql.Select<User>().AsTable((p1,p2)=> "siteserver_User").Where(p => p.SiteId == request.SiteId)
                .WhereIf(!string.IsNullOrWhiteSpace(request.Keyword),p=> p.UserName.Contains(request.Keyword) || p.Email.Contains(request.Keyword) || p.Mobile.Contains(request.Keyword) || p.DisplayName.Contains(request.Keyword))
                .OrderBy(!string.IsNullOrWhiteSpace(request.Order),$"{request.Order} desc")
                .Count(out var count).Page(request.Offset/request.Limit+1, request.Limit).ToList();
            //var users = await _userRepository.GetUsersAsync(request.State, request.GroupId, request.LastActivityDate, request.Keyword, request.Order, request.Offset, request.Limit);

            return new GetResults
            {
                Users = users,
                Count = (int)count,
                Groups = groups
            };
        }
      public  class SiteGetRequest: GetRequest
        { 
            public int SiteId { get; set; }
        }
    }
}
