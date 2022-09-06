using Datory;
using Datory.Annotations;

namespace SSCMS.Models
{
    [DataTable(Configuration.ESets.CMSDbPrefix+"_AdministratorsInRoles")]
    public class AdministratorsInRoles : Entity
    {
        [DataColumn]
        public string RoleName { get; set; }

        [DataColumn]
        public string UserName { get; set; }
	}
}
