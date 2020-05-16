using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineShoppingBackend.Models;
using OnlineShoppingBackend.Utils;
using SqlSugar;

namespace OnlineShoppingBackend.DAL
{
    public class OrderDAL : DbContext
    {
        /// <summary>
        /// 根据商品 ID 查找商品
        /// </summary>
        /// <param name="id">商品 ID</param>
        private Item getItemById(string id)
        {
            if (id == null)
            {
                return null;
            }
            ItemDAL itemDal = new ItemDAL();
            var result = itemDal.getItemById(id);
            return result;
        }

        /// <summary>
        /// 获取订单的商品和数量列表
        /// </summary>
        /// <param name="order">订单对象</param>
        public List<OrderItem> getItemsForOrder(Order order)
        {
            var result = db.Queryable<OrderItem>()
                            .Where(p => p.orderId == order.orderId)
                            .Mapper(it => it.item = getItemById(it.itemId))
                            .ToList();
            return result;
        }

        /// <summary>
        /// 根据用户 ID 获取指定用户的所有订单，按创建时间倒序
        /// </summary>
        /// <param name="userId">用户 ID</param>
        /// <param name="closed">获取已关闭的订单</param>
        /// <param name="pageIndex">页码编号（从 1 开始）</param>
        /// <param name="pageSize">每页订单数量</param>
        public List<Order> listOrdersByUserId(string userId, bool closed, int pageIndex, int pageSize)
        {
            var result = db.Queryable<Order>()
                            .Where(p => p.userId == userId)
                            .Where(p => p.close == closed)
                            .Mapper(it => it.items = getItemsForOrder(it))
                            .OrderBy(p => p.createTime, OrderByType.Desc)
                            .ToPageList(pageIndex, pageSize);
            return result;
        }

        /// <summary>
        /// 根据订单 ID 查找订单
        /// </summary>
        /// <param name="id">订单 ID</param>
        public Order getOrderById(string id)
        {
            if (id == null)
            {
                return null;
            }
            var result = db.Queryable<Order>().Mapper(it => it.items = getItemsForOrder(it)).InSingle(id);
            return result;
        }

        /// <summary>
        /// 添加订单
        /// </summary>
        /// <param name="order">订单对象</param>
        /// <returns>数据库受影响的行数</returns>
        public int addOrder(Order order)
        {
            var result = db.Insertable<Order>(order).ExecuteCommand(); // 订单表添加
            db.Insertable<OrderItem>(order.items).ExecuteCommand(); // 订单商品表添加
            return result;
        }

        /// <summary>
        /// 更新订单信息
        /// </summary>
        /// <param name="order">订单对象</param>
        /// <returns>数据库受影响的行数</returns>
        public int updateOrder(Order order)
        {
            var result = db.Updateable<Order>(order)
                            .ExecuteCommand();
            return result;
        }
    }
}
