﻿using Datory;
using Datory.Annotations;

namespace SSCMS.Models
{
    [DataTable(Configuration.ESets.CMSDbPrefix+"_WxReplyKeyword")]
    public class WxReplyKeyword : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public int RuleId { get; set; }

        [DataColumn]
        public bool Exact { get; set; }

        [DataColumn]
        public string Text { get; set; }
    }
}