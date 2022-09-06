﻿using Datory;
using Datory.Annotations;

namespace SSCMS.Models
{
    [DataTable(Configuration.ESets.CMSDbPrefix+"_MaterialFile")]
    public class MaterialFile : Entity
    {
        [DataColumn]
        public string MediaId { get; set; }

        [DataColumn]
        public int GroupId { get; set; }

        [DataColumn]
        public string Title { get; set; }

        [DataColumn]
        public string FileType { get; set; }

        [DataColumn]
        public string Url { get; set; }
    }
}