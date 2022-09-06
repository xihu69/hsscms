using ELibrary.Common.BaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELibrary.Models
{
    /// <summary>
    /// 评分
    /// </summary>
    [Table("eb_ContentScore")]
    public class ContentScore : ReaderRecordBase
    {
        public decimal Score { get; set; }

    }

}
