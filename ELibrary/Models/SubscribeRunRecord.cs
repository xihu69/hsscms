using ELibrary.Common.BaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELibrary.Models
{
    /// <summary>
    /// 订阅获取记录
    /// </summary>
    public class SubscribeRunRecord:Entity
    {
        public string FromSite { get; set; }
        public string ToSite { get; set; }
        public string ChannelGid { get; set; }
        /// <summary>
        /// 查询范围：开始时间
        /// </summary>
        public DateTime QueryBegin { get; set; }
        public DateTime QueryEnd { get; set; }
        /// <summary>
        /// 批次标识
        /// </summary>
        public DateTime RunTime { get; set; }
        public string Remark { get; set; }

        /// <summary>
        /// 0,待确认，1已确认，2执行中
        /// </summary>
        public int State { get; set; }
    }
    /// <summary>
    /// 内容拉取记录
    /// </summary>
    public class PullRecord: Entity
    {
        public string ContentGid { get; set; }
        public DateTime Newstime { get; set; }
        public int State { get; set; }
        public int RunNum { get;set; }
        public DateTime? RunTime { get; set; }
        public string Remark { get; set; }

        public long RunRecordId { get; set; }

    }
}
