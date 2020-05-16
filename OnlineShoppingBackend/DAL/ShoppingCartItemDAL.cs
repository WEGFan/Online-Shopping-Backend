using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineShoppingBackend.Models;
using OnlineShoppingBackend.Utils;
using SqlSugar;

namespace OnlineShoppingBackend.DAL
{
    public class ShoppingCartItemDAL : DbContext
    {
        /// <summary>
        /// 根据商品 ID 查找商品
        /// </summary>
        /// <param name="id">商品 ID</param>
        private Item getItemById(string id)
        {
            ItemDAL itemDal = new ItemDAL();
            var result = itemDal.getItemById(id);
            return result;
        }

        /// <summary>
        /// 根据用户 ID 获取指定用户的购物车商品
        /// </summary>
        public List<ShoppingCartItem> listShoppingCartItemsByUserId(string userId)
        {
            var result = db.Queryable<ShoppingCartItem>()
                            .Where(p => p.userId == userId)
                            .Mapper(it => it.item = getItemById(it.itemId))
                            .OrderBy(p => p.createTime, OrderByType.Desc)
                            .ToList();
            return result;
        }

        /// <summary>
        /// 根据用户 ID 和商品 ID 获取购物车项目
        /// </summary>
        public ShoppingCartItem getShoppingCartItemByUserIdAndItemId(string userId, string itemId)
        {
            var result = db.Queryable<ShoppingCartItem>()
                            .Where(p => p.userId == userId)
                            .Where(p => p.itemId == itemId)
                            .First();
            return result;
        }

        /// <summary>
        /// 添加购物车商品
        /// </summary>
        /// <param name="shoppingCartItem">购物车商品对象</param>
        /// <returns>数据库受影响的行数</returns>
        public int addShoppingCartItem(ShoppingCartItem shoppingCartItem)
        {
            var previous = db.Queryable<ShoppingCartItem>()
                                .Where(p => p.userId == shoppingCartItem.userId)
                                .Where(p => p.itemId == shoppingCartItem.itemId)
                                .First();
            // 如果购物车已存在该商品则累加数量并更新
            if (previous != null)
            {
                shoppingCartItem.count += previous.count;
                return updateShoppingCartItem(shoppingCartItem);
            }
            var result = db.Insertable<ShoppingCartItem>(shoppingCartItem)
                            .ExecuteCommand();
            return result;
        }

        /// <summary>
        /// 更新购物车商品
        /// </summary>
        /// <param name="shoppingCartItem">购物车商品对象</param>
        /// <returns>数据库受影响的行数</returns>
        public int updateShoppingCartItem(ShoppingCartItem shoppingCartItem)
        {
            var result = db.Updateable<ShoppingCartItem>(shoppingCartItem)
                            .Where(p => p.userId == shoppingCartItem.userId)
                            .Where(p => p.itemId == shoppingCartItem.itemId)
                            .UpdateColumns(p => new { p.count })
                            .ExecuteCommand();
            return result;
        }

        /// <summary>
        /// 删除购物车商品
        /// </summary>
        /// <param name="order">购物车商品对象</param>
        /// <returns>数据库受影响的行数</returns>
        public int deleteShoppingCartItem(ShoppingCartItem shoppingCartItem)
        {
            var result = db.Deleteable<ShoppingCartItem>()
                            .Where(p => p.userId == shoppingCartItem.userId)
                            .Where(p => p.itemId == shoppingCartItem.itemId)
                            .ExecuteCommand();
            return result;
        }
    }
}
