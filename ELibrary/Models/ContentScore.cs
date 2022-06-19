using ELibrary.Common.BaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELibrary.Models
{
    /// <summary>
    /// 评分
    /// </summary>
    public class ContentScore : ReaderRecordBase
    {
        public decimal Score { get; set; }

    }

}
