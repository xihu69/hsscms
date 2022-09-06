using Datory;
using Datory.Annotations;

namespace SSCMS.Models
{
    [DataTable(Configuration.ESets.CMSDbPrefix+"_PluginConfig")]
    public class PluginConfig : Entity
	{
        [DataColumn]
        public string PluginId { get; set; }

        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public string ConfigName { get; set; }

        [DataColumn(Text = true)]
        public string ConfigValue { get; set; }
	}
}
