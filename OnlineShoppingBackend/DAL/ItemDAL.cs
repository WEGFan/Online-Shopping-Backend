using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineShoppingBackend.Models;
using OnlineShoppingBackend.Utils;
using SqlSugar;

namespace OnlineShoppingBackend.DAL
{
    public class ItemDAL : DbContext
    {
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
        /// 查询该商品是否被收藏
        /// </summary>
        /// <param name="userId">用户 ID</param>
        /// <param name="item">商品对象</param>
        private bool checkItemCollected(string userId, Item item)
        {
            if (userId == null)
            {
                return false;
            }
            var result = db.Queryable<FavoriteItem>()
                            .Where(p => p.userId == userId)
                            .Where(p => p.itemId == item.itemId)
                            .Any();
            return result;
        }

        /// <summary>
        /// 获取商品
        /// </summary>
        /// <param name="onlyOpened">只获取上架商品</param>
        /// <param name="orderBy">排序方式</param>
        /// <param name="orderAscending">是否升序</param>
        /// <param name="shopId">店铺 ID</param>
        /// <param name="keywordList">搜索关键词列表</param>
        /// <param name="pageIndex">页码编号（从 1 开始）</param>
        /// <param name="pageSize">每页商品数量</param>
        public List<Item> listItems(bool onlyOpened, List<string> keywordList, string shopId, string orderBy, bool orderAscending, int pageIndex, int pageSize)
        {
            var query = db.Queryable<Item>();

            // 只获取上架商品
            if (onlyOpened)
            {
                query.Where(p => p.open == true);
            }

            // 只获取该店铺的商品
            if (shopId != null)
            {
                query.Where(p => p.shopId == shopId);
            }

            // 名称关键词
            var itemFilterByName = db.Queryable<Item>();
            foreach (string keyword in keywordList)
            {
                itemFilterByName.Where(p => p.name.Contains(keyword));
            }

            // 标签关键词
            var tagQuery = db.Queryable<Tag>();
            foreach (string keyword in keywordList)
            {
                tagQuery.Where(p => p.name.Contains(keyword));
            }
            var tagIdList = tagQuery.Select(p => p.tagId).ToList();
            var itemIdFilterByTag = db.Queryable<ItemTag>()
                                .In(p => p.tagId, tagIdList)
                                .Select(p => p.itemId);
            var itemFilterByTag = db.Queryable<Item>()
                                .In(p => p.itemId, itemIdFilterByTag);

            var itemIdList = db.Union(itemFilterByName, itemFilterByTag).Select(p => p.itemId).ToList();

            query.In(p => p.itemId, itemIdList);

            // 排序
            query.OrderBy($"{orderBy} {(orderAscending ? "asc" : "desc")}");

            var result = query.Mapper(p => p.tags = getTagNameForItem(p))
                                .ToPageList(pageIndex, pageSize);
            return result;
        }

        /// <summary>
        /// 随机选择商品
        /// </summary>
        /// <param name="number">数量</param>
        public List<Item> randomChooseItems(int number)
        {
            Random random = new Random();
            var randomItemIdList = db.Queryable<Item>()
                                .Select(p => p.itemId)
                                .ToList();
            int count = randomItemIdList.Count;
            randomItemIdList.Sort((a, b) => random.Next(-1, 1));
            if (randomItemIdList.Count > number)
            {
                randomItemIdList = randomItemIdList.GetRange(random.Next(0, count - number - 1), number);
            }
            var result = db.Queryable<Item>()
                            .In(p => p.itemId, randomItemIdList)
                            .Mapper(p => p.tags = getTagNameForItem(p))
                            .ToList();
            return result;
        }

        /// <summary>
        /// 根据商品 ID 查找商品
        /// </summary>
        /// <param name="userId">用户 ID</param>
        /// <param name="itemId">商品 ID</param>
        public Item getItemById(string itemId, string userId = null)
        {
            var result = db.Queryable<Item>()
                            .Mapper(p => p.tags = getTagNameForItem(p))
                            .Mapper(p => p.collected = checkItemCollected(userId, p))
                            .InSingle(itemId);
            return result;
        }

        /// <summary>
        /// 添加商品
        /// </summary>
        /// <param name="item">商品对象</param>
        /// <returns>数据库受影响的行数</returns>
        public int addItem(Item item)
        {
            var result = db.Insertable<Item>(item).ExecuteCommand();
            return result;
        }

        /// <summary>
        /// 更新商品
        /// </summary>
        /// <param name="item">商品对象</param>
        /// <returns>数据库受影响的行数</returns>
        public int updateItem(Item item)
        {
            var result = db.Updateable<Item>(item).ExecuteCommand();
            return result;
        }
    }
}
