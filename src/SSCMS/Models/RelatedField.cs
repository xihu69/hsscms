using Datory;
using Datory.Annotations;

namespace SSCMS.Models
{
    [DataTable(Configuration.ESets.CMSDbPrefix+"_RelatedField")]
    public class RelatedField : Entity
	{
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
	      public string Title { get; set; }
	}
}
