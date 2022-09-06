using Datory;
using Datory.Annotations;

namespace SSCMS.Models
{
    [DataTable(Configuration.ESets.CMSDbPrefix+"_DbCache")]
	public class DbCache : Entity
    {
        [DataColumn]
        public string CacheKey { get; set; }

        [DataColumn]
        public string CacheValue { get; set; }
    }
}
