﻿using Datory;
using Datory.Annotations;
using SSCMS.Enums;

namespace SSCMS.Models
{
    [DataTable(Configuration.ESets.CMSDbPrefix+"_Stat")]
    public class Stat : Entity
    {
        [DataColumn]
        public StatType StatType { get; set; }

        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public int AdminId { get; set; }

        [DataColumn]
        public int Count { get; set; }
    }
}
