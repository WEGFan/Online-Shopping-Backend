using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineShoppingBackend.Models;
using OnlineShoppingBackend.Utils;
using SqlSugar;

namespace OnlineShoppingBackend.DAL
{
    public class FavoriteItemDAL : DbContext
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
        /// 获取商品的标签名数组
        /// </summary>
        /// <param name="item">商品对象</param>
        /// <returns></returns>
        private List<string> getTagNameForItem(Item item)
        {
            var tagIdList = db.Queryable<ItemTag>()
                                .Where(p => p.itemId == item.itemId)
                                .Select(p => p.tagId)
                                .ToList();
            var tagNameList = db.Queryable<Tag>()
                                .In(p => p.tagId, tagIdList)
                                .Select(p => p.name)
                                .ToList();
            return tagNameList;
        }

        /// <summary>
        /// 根据用户 ID 获取用户的收藏商品列表
        /// </summary>
        public List<Item> listFavoriteItemsByUserId(string userId, int pageIndex, int pageSize)
        {
            var itemIdList = db.Queryable<FavoriteItem>()
                                .Where(p => p.userId == userId)
                                .Select(p => p.itemId)
                                .ToList();
            var result = db.Queryable<Item>()
                            .In(p => p.itemId, itemIdList)
                            .Mapper(p => p.tags = getTagNameForItem(p))
                            .ToPageList(pageIndex, pageSize);
            return result;
        }

        /// <summary>
        /// 切换收藏商品项目
        /// </summary>
        /// <returns>是否增加</returns>
        public bool toggleFavoriteItem(string userId, string itemId)
        {
            var previous = db.Queryable<FavoriteItem>()
                                .Where(p => p.userId == userId)
                                .Where(p => p.itemId == itemId)
                                .First();

            // 如果收藏项目已经存在则删除
            if (previous != null)
            {
                db.Deleteable<FavoriteItem>()
                    .Where(p => p.userId == userId)
                    .Where(p => p.itemId == itemId)
                    .ExecuteCommand();
                return false;
            }
            // 否则增加
            db.Insertable<FavoriteItem>(new { userId = userId, itemId = itemId })
                .ExecuteCommand();
            return true;
        }
    }
}
