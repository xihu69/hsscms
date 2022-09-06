using System.Collections.Generic;
using Datory;
using Datory.Annotations;

namespace SSCMS.Models
{
    [DataTable(Configuration.ESets.CMSDbPrefix+"_MaterialMessage")]
    public class MaterialMessage : Entity
    {
        [DataColumn]
        public string MediaId { get; set; }

        [DataColumn]
        public int GroupId { get; set; }

        [DataIgnore]
        public List<MaterialMessageItem> Items { get; set; }
    }
}