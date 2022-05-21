using ELibrary.Common.BaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELibrary.Models
{
     [Table("eb_taskRecord")]
    public class TaskRecord : EntityAdd
    { 
        /// <summary> 1导出，0导入
        /// 
        /// </summary>
        public int Type { get; set; }
        /// <summary>导出原始id清单，导入文件名称
        /// 
        /// </summary>
        public string InputData { get; set; }
        /// <summary>导出文件名称
        /// 
        /// </summary>
        public string Result { get; set; }
        public string Title { get; set; }
        public string StepName { get; set; }
        public string StepData { get; set; }
        /// <summary>0未处理，1列表文件处理中，2列表文件已生成
        /// 
        /// </summary>
        public int State { get; set; }
        public string Info { get; set; }
    }
}
