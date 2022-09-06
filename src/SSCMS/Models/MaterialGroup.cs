using Datory;
using Datory.Annotations;
using SSCMS.Enums;

namespace SSCMS.Models
{
    [DataTable(Configuration.ESets.CMSDbPrefix+"_MaterialGroup")]
    public class MaterialGroup : Entity
    {
        [DataColumn]
        public MaterialType MaterialType { get; set; }

        [DataColumn]
        public string GroupName { get; set; }
    }
}
