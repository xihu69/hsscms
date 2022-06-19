using ELibrary.Common.BaseModel;
using ELibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELibrary.Models
{
    public class ReaderRecordBase:EntityUpdate
    {
        public long ContentId { get; set; }
        public string ContentGid { get; set; }
        public string ContentName { get; set; }
        public string SiteGid { get; set; }
        public long SiteId { get; set; }
        public long UserId { get; set; }
    }

    /// <summary>
    /// 阅读记录，下载记录
    /// </summary>
    public class ReaderRecord: ReaderRecordBase
    {
        /// <summary>
        /// 1阅读，2下载
        /// </summary>
        public int Type { get; set; }
    }
}
