using System.Collections.Generic;
using Datory;
using Datory.Annotations;

namespace SSCMS.Models
{
    [DataTable(Configuration.ESets.CMSDbPrefix+"_PermissionsInRoles")]
    public class PermissionsInRoles : Entity
    {
        [DataColumn]
        public string RoleName { get; set; }

        [DataColumn(Text = true)]
        public List<string> AppPermissions { get; set; }
    }
}