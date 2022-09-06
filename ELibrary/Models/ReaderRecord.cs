using ELibrary.Common.BaseModel;
using ELibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELibrary.Models
{
    public class ReaderRecordBase:EntityUpdate
    {
        public long? ContentId { get; set; }
        public string? ContentGid { get; set; }
        //名称、访问的url
        public string? Content { get; set; }
        public string? SiteGid { get; set; }
        public long SiteId { get; set; }
        public long? UserId { get; set; }

        public string? Ip { get; set; }
    }

    /// <summary>
    /// 阅读记录，下载记录
    /// </summary>
   [Table("eb_ReaderRecord")]
    public class ReaderRecord: ReaderRecordBase
    {
        /// <summary>
        /// 1阅读，2下载，0访问
        /// </summary>
        public int Type { get; set; }
    }
}
