using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineShoppingBackend.Models;
using OnlineShoppingBackend.Utils;
using SqlSugar;

namespace OnlineShoppingBackend.DAL
{
    public class FollowShopDAL : DbContext
    {
        /// <summary>
        /// 根据店铺 ID 查找店铺
        /// </summary>
        /// <param name="id">店铺 ID</param>
        private Shop getShopById(string id)
        {
            var result = db.Queryable<Shop>().InSingle(id);
            return result;
        }

        /// <summary>
        /// 根据用户 ID 获取用户的关注店铺列表
        /// </summary>
        public List<Shop> listFollowShopsByUserId(string userId, int pageIndex, int pageSize)
        {
            var shopIdList = db.Queryable<FollowShop>()
                                .Where(p => p.userId == userId)
                                .Select(p => p.shop)
                                .ToList();
            var result = db.Queryable<Shop>()
                            .In(p => p.shopId, shopIdList)
                            .ToPageList(pageIndex, pageSize);
            return result;
        }

        /// <summary>
        /// 切换收藏商品项目
        /// </summary>
        /// <returns>是否增加</returns>
        public bool toggleFollowShop(string userId, string shopId)
        {
            var previous = db.Queryable<FollowShop>()
                                .Where(p => p.userId == userId)
                                .Where(p => p.shopId == shopId)
                                .First();

            // 如果收藏项目已经存在则删除
            if (previous != null)
            {
                db.Deleteable<FollowShop>()
                    .Where(p => p.userId == userId)
                    .Where(p => p.shopId == shopId)
                    .ExecuteCommand();
                return false;
            }
            // 否则增加
            db.Insertable<FollowShop>(new { userId = userId, shopId = shopId })
                .ExecuteCommand();
            return true;
        }
    }
}
