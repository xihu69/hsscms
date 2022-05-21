using FreeSql.DataAnnotations;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ELibrary.Common.BaseModel
{
    public interface IEntityAdd
    {
        //long? CreatedUserId { get; set; }
        //string CreatedUserName { get; set; }
        //DateTime? CreatedTime { get; set; }
        /// <summary>
        /// 创建者Id
        /// </summary>
        [Description("创建者Id")]
        [Column(Position = -3, CanUpdate = false)]
        public long? CreatedUserId { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        [Description("创建者")]
        [Column(Position = -2, CanUpdate = false), MaxLength(50)]
        public string CreatedUserName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        [Column(Position = -1, CanUpdate = false, ServerTime = DateTimeKind.Local)]
        public DateTime? CreatedTime { get; set; }
    }
    /// <summary>
    /// 实体创建
    /// </summary>
    public class EntityAdd<TKey> : Entity<TKey>, IEntityAdd
    {
        /// <summary>
        /// 创建者Id
        /// </summary>
        [Description("创建者Id")]
        [Column(Position = -3, CanUpdate = false)]
        public long? CreatedUserId { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        [Description("创建者")]
        [Column(Position = -2, CanUpdate = false), MaxLength(50)]
        public string CreatedUserName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        [Column(Position = -1, CanUpdate = false, ServerTime = DateTimeKind.Local)]
        public DateTime? CreatedTime { get; set; }
       }

    public class EntityAdd : EntityAdd<long>
    {

    }
}
