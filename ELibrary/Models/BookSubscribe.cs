using ELibrary.Common.BaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELibrary.Models
{
    /// <summary>
    /// 服务器保存的订阅基本信息
    /// </summary>
    public class BookSubscribe: EntityUpdate
    {
        /// <summary>
        /// 0未启用，1已启用
        /// </summary>
       public int State { get; set; }
        public string FromSite { get; set; }
        /// <summary>
        /// 分隔符字符串
        /// </summary>
        public string ChannelGids { get; set; }

        public string ToStie { get; set; }

        public DateTime LastRunTime { get;set; }
        public string LastRunInfo { get; set; }
    }
     
}
