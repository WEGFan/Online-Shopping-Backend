using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineShoppingBackend.Models;
using OnlineShoppingBackend.Utils;
using SqlSugar;

namespace OnlineShoppingBackend.DAL
{
    public class ShopDAL : DbContext
    {
        /// <summary>
        /// 获取店铺
        /// </summary>
        /// <param name="onlyOpened">只获取开启店铺</param>
        /// <param name="orderBy">排序方式</param>
        /// <param name="orderAscending">是否升序</param>
        /// <param name="keywordList">搜索关键词列表</param>
        /// <param name="pageIndex">页码编号（从 1 开始）</param>
        /// <param name="pageSize">每页店铺数量</param>
        public List<Shop> listShops(bool onlyOpened, List<string> keywordList, string orderBy, bool orderAscending, int pageIndex, int pageSize)
        {
            var query = db.Queryable<Shop>();

            // 只获取开启店铺
            if (onlyOpened)
            {
                query.Where(p => p.open == true);
            }

            // 搜索关键词
            foreach (string keyword in keywordList)
            {
                query.Where(p => p.name.Contains(keyword));
            }

            // 排序
            query.OrderBy($"{orderBy} {(orderAscending ? "asc" : "desc")}");

            var result = query.ToPageList(pageIndex, pageSize);
            return result;
        }

        /// <summary>
        /// 根据店铺 ID 查找店铺
        /// </summary>
        /// <param name="id">店铺 ID</param>
        public Shop getShopById(string id)
        {
            var result = db.Queryable<Shop>().InSingle(id);
            return result;
        }

        /// <summary>
        /// 添加店铺
        /// </summary>
        /// <param name="shop">店铺对象</param>
        /// <returns>数据库受影响的行数</returns>
        public int addShop(Shop shop)
        {
            var result = db.Insertable<Shop>(shop).ExecuteCommand();
            return result;
        }

        /// <summary>
        /// 更新店铺
        /// </summary>
        /// <param name="tag">店铺对象</param>
        /// <returns>数据库受影响的行数</returns>
        public int updateShop(Shop shop)
        {
            var result = db.Updateable<Shop>(shop).ExecuteCommand();
            return result;
        }
    }
}
