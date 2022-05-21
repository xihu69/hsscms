using FreeSql.DataAnnotations;
using System.ComponentModel;

namespace ELibrary.Common.BaseModel
{
    public interface IEntityBase
    {
        object GetId();
        void SetId(object id);
    }
    public interface IEntity<TKey>
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        TKey Id { get; set; }
    }

    public interface IEntity : IEntity<long>
    {
    }


    public class Entity<TKey> : IEntity<TKey>, IEntityBase
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Description("主键Id")]
        [Column(Position = 1, IsIdentity = true)]
        public virtual TKey Id { get; set; }

        public object GetId()
        {
            return Id;
        }

        public void SetId(object id)
        {
            Id = (TKey)id;
        }
    }

    public class Entity : Entity<long>
    {

    }
}
