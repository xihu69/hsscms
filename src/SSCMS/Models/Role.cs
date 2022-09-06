using Datory;
using Datory.Annotations;

namespace SSCMS.Models
{
    [DataTable(Configuration.ESets.CMSDbPrefix+"_Role")]
    public class Role : Entity
    {
        [DataColumn]
        public string RoleName  { get; set; }

        [DataColumn]
        public string CreatorUserName { get; set; }

        [DataColumn]
        public string Description { get; set; }
    }
}
