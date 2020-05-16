using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineShoppingBackend.Models;
using OnlineShoppingBackend.Utils;
using SqlSugar;

namespace OnlineShoppingBackend.DAL
{
    public class UserDAL : DbContext
    {
        /// <summary>
        /// 根据用户 ID 查找用户
        /// </summary>
        /// <param name="id">用户 ID</param>
        public User getUserById(string id)
        {
            if (id == null)
            {
                return null;
            }
            var result = db.Queryable<User>()
                            .InSingle(id);
            return result;
        }

        /// <summary>
        /// 根据帐号和密码查找用户
        /// </summary>
        /// <param name="account">帐号</param>
        /// <param name="password">密码</param>
        /// <returns>用户对象</returns>
        public User getUserByAccount(string account)
        {
            if (account == null)
            {
                return null;
            }
            var result = db.Queryable<User>()
                            .Where(p => p.account == account)
                            .First();
            return result;
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="user">用户对象</param>
        /// <returns>数据库受影响的行数</returns>
        public int addUser(User user)
        {
            var result = db.Insertable<User>(user)
                            .ExecuteCommand();
            return result;
        }

        /// <summary>
        /// 检查帐号名是否已存在
        /// </summary>
        /// <param name="account">帐号名</param>
        /// <returns>帐号名是否已存在</returns>
        public bool checkAccountExists(string account)
        {
            if (account == null)
            {
                return false;
            }

            var result = db.Queryable<User>()
                            .Where(p => p.account == account)
                            .First();
            return result != null;
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="user">用户对象</param>
        /// <returns>数据库受影响的行数</returns>
        public int updateUserInfo(User user)
        {
            var result = db.Updateable<User>(user)
                            .ExecuteCommand();
            return result;
        }

        /// <summary>
        /// 更新用户密码
        /// </summary>
        /// <param name="user">用户对象</param>
        /// <returns>数据库受影响的行数</returns>
        public int updateUserPassword(User user)
        {
            var result = db.Updateable<User>(user)
                            .UpdateColumns(new string[] { "password" })
                            .ExecuteCommand();
            return result;
        }
    }
}
