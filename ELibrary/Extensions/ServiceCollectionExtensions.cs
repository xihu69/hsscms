using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELibrary.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddFreeSql(this IServiceCollection services, string databaseType, string connectionString)
        {
            var type = Enum.Parse<FreeSql.DataType>(databaseType, true);
            IFreeSql fsql = new FreeSql.FreeSqlBuilder()
              .UseConnectionString(type, connectionString)
              .UseAutoSyncStructure(false) //自动同步实体结构到数据库，FreeSql不会扫描程序集，只有CRUD时才会生成表。
              .Build();
            services.AddSingleton(fsql);
        }
    }
}
