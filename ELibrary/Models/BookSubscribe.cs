using ELibrary.Common.BaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELibrary.Models
{
    public class BookSubscribe: EntityAdd
    {
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
