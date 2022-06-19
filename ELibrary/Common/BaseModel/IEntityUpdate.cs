using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FreeSql.DataAnnotations;
namespace ELibrary.Common.BaseModel
{
    public interface IEntityUpdate
    {
        long? ModifiedUserId { get; set; }
        string ModifiedUserName { get; set; }
        DateTime? ModifiedTime { get; set; }
    }
    // <summary>
    /// 实体修改
    /// </summary>
    public class EntityUpdate<TKey> : Entity<TKey>, IEntityUpdate
    {
        /// <summary>
        /// 修改者Id
        /// </summary>
        [Description("修改者Id")]
        [Column(Position = -3)]
        public long? ModifiedUserId { get; set; }

        /// <summary>
        /// 修改者
        /// </summary>
        [Description("修改者")]
        [Column(Position = -2), MaxLength(50)]
        public string ModifiedUserName { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [Description("修改时间")]
        [Column(Position = -1,  ServerTime = DateTimeKind.Local)]
        public DateTime? ModifiedTime { get; set; }
    }

    public class EntityUpdate : EntityUpdate<long>
    {

    }
}
