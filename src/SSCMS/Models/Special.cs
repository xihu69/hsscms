using System;
using Datory;
using Datory.Annotations;

namespace SSCMS.Models
{
    [DataTable(Configuration.ESets.CMSDbPrefix+"_Special")]
    public class Special : Entity
	  {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public string Title { get; set; }

        [DataColumn]
        public string Url { get; set; }

        [DataColumn]
        public DateTime AddDate { get; set; }
    }
}
