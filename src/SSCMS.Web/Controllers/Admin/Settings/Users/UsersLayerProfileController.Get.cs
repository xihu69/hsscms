using System.Linq;
using System.Threading.Tasks;
using ELibrary.Utils;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersLayerProfileController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] int userId)
        {
            //!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsers,PowerSign.Site.settings_users)
            if (_authManager.AdminId<1)
            {
                return Unauthorized();
            }

            var user = await _userRepository.GetByUserIdAsync(userId);
            var userStyles = await _tableStyleRepository.GetUserStylesAsync();
            var groups = await _userGroupRepository.GetUserGroupsAsync();
            var styles = userStyles.Select(x => new InputStyle(x));

            return new GetResult
            {
                User = user,
                Groups = groups,
                Styles = styles
            };
        }
    }
}
