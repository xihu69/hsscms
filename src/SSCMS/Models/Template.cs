using Datory;
using Datory.Annotations;
using SSCMS.Enums;

namespace SSCMS.Models
{
    [DataTable(Configuration.ESets.CMSDbPrefix+"_Template")]
    public class Template : Entity
    {
        [DataColumn] 
        public int SiteId { get; set; }

        [DataColumn] 
        public string TemplateName { get; set; }

        [DataColumn] 
        public TemplateType TemplateType { get; set; }

        [DataColumn] 
        public string RelatedFileName { get; set; }

        [DataColumn] 
        public string CreatedFileFullName { get; set; }

        [DataColumn] 
        public string CreatedFileExtName { get; set; }

        [DataColumn]
        public bool DefaultTemplate { get; set; }

        [DataIgnore]
        public string Content { get; set; }
    }
}