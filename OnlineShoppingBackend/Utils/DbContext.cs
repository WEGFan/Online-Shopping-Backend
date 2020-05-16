using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SqlSugar;

namespace OnlineShoppingBackend.Utils
{
    public class DbContext
    {
        public SqlSugarClient db;

        public DbContext()
        {
            db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = Startup.Configuration["AppSettings:MySql:ConnectionString"],
                DbType = DbType.MySql,
                InitKeyType = InitKeyType.Attribute, // 从特性读取主键和自增列信息
                IsAutoCloseConnection = true,

            });
            // 调式代码 用来打印SQL 
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                Debug.WriteLine(sql + "\r\n" +
                    db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                Debug.WriteLine("");
            };
        }
    }
}
