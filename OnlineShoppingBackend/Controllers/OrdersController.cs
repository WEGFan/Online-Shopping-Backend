using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OnlineShoppingBackend.DAL;
using OnlineShoppingBackend.Models;
using OnlineShoppingBackend.Utils;
using HttpStatusCodes = Microsoft.AspNetCore.Http.StatusCodes;
using StatusCodes = OnlineShoppingBackend.Utils.StatusCodes;

namespace OnlineShoppingBackend.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrdersController : Controller
    {
        private readonly ILogger<OrdersController> logger;

        public OrdersController(ILogger<OrdersController> logger)
        {
            this.logger = logger;
        }

        private const int pageSize = 8; // 每页显示数量

        // 获取用户的所有订单
        // GET /api/order/list?page=[int]&closed=[bool]
        [HttpGet("list")]
        public JsonResult listOrders([FromQuery]int page = 1, [FromQuery]bool closed = true)
        {
            // 登录验证
            string sessionUserId = HttpContext.Session.GetString("userId");
            string sessionPassword = HttpContext.Session.GetString("password");
            UserDAL userDal = new UserDAL();
            if (sessionUserId == null || userDal.getUserById(sessionUserId)?.password != sessionPassword)
            {
                return new JsonResult(Return.Error("用户未登录或无权限访问", HttpStatusCodes.Status401Unauthorized));
            }

            OrderDAL orderDal = new OrderDAL();
            List<Order> result = orderDal.listOrdersByUserId(sessionUserId, closed, page, pageSize);
            return new JsonResult(Return.Success(result));
        }

        // 获取订单 ID 为 {id} 的订单
        // GET /api/order/info?orderId={id}
        [HttpGet("info")]
        public JsonResult getOrderInfo([FromQuery]string order_id)
        {
            string sessionUserId = HttpContext.Session.GetString("userId");
            string sessionPassword = HttpContext.Session.GetString("password");

            UserDAL userDal = new UserDAL();
            OrderDAL orderDal = new OrderDAL();

            Order result = orderDal.getOrderById(order_id);

            // 权限验证
            if (sessionUserId == null || userDal.getUserById(sessionUserId)?.password != sessionPassword ||
                result?.userId != sessionUserId)
            {
                return new JsonResult(Return.Error("用户未登录或无权限访问", HttpStatusCodes.Status401Unauthorized));
            }

            if (result == null)
            {
                return new JsonResult(Return.Error("找不到此订单", StatusCodes.OrderNotFound));
            }

            return new JsonResult(Return.Success(result));
        }

        // 创建订单
        // POST /api/order/create
        [HttpPost("create")]
        public JsonResult createOrder([FromBody]Order order)
        {
            // 登录验证
            string sessionUserId = HttpContext.Session.GetString("userId");
            string sessionPassword = HttpContext.Session.GetString("password");
            UserDAL userDal = new UserDAL();
            if (sessionUserId == null || userDal.getUserById(sessionUserId)?.password != sessionPassword)
            {
                return new JsonResult(Return.Error("用户未登录或无权限访问", HttpStatusCodes.Status401Unauthorized));
            }

            ShoppingCartItemDAL shoppingCartItemDal = new ShoppingCartItemDAL();
            ItemDAL itemDal = new ItemDAL();
            OrderDAL orderDal = new OrderDAL();

            order.orderId = Guid.NewGuid().ToString();
            order.userId = sessionUserId;
            order.createTime = DateTime.Now;
            order.deliveryTime = order.paymentTime = order.receiptTime = null;
            order.price = 0;

            // 模型验证
            ModelState.Clear();
            TryValidateModel(order);
            if (!ModelState.IsValid)
            {
                return new JsonResult(Return.ModelError(ModelState));
            }

            for (int i = 0; i < order.items.Count; i++)
            {
                order.items[i].item = itemDal.getItemById(order.items[i].itemId); // 获取商品对象
                if (order.items[i].item?.open != true)
                {
                    return new JsonResult(Return.Error("找不到此商品或此商品已下架", StatusCodes.ItemNotFound));
                }
                if (order.items[i].count > order.items[i].item.quantity)
                {
                    return new JsonResult(Return.Error("库存量不足", StatusCodes.NotEnoughItems));
                }

                order.items[i].orderId = order.orderId;
                order.price += order.items[i].count * order.items[i].item.price; // 计算价格
            }

            foreach (OrderItem item in order.items)
            {
                // 删除购物车里对应的物品
                shoppingCartItemDal.deleteShoppingCartItem(new ShoppingCartItem
                {
                    itemId = item.itemId,
                    userId = sessionUserId
                });

                // 减少库存量
                item.item.quantity -= item.count;
                itemDal.updateItem(item.item);
            }

            int result = orderDal.addOrder(order);
            return new JsonResult(Return.Success(order));
        }

        // 订单付款
        // POST /api/order/pay
        [HttpPost("pay")]
        public JsonResult payOrder([FromBody]JObject data)
        {
            string sessionUserId = HttpContext.Session.GetString("userId");
            string sessionPassword = HttpContext.Session.GetString("password");

            UserDAL userDal = new UserDAL();
            OrderDAL orderDal = new OrderDAL();
            ItemDAL itemDal = new ItemDAL();

            string orderId = data.Value<string>("order_id");
            Order order = orderDal.getOrderById(orderId);

            // 权限验证
            if (sessionUserId == null || userDal.getUserById(sessionUserId)?.password != sessionPassword ||
                order?.userId != sessionUserId)
            {
                return new JsonResult(Return.Error("用户未登录或无权限访问", HttpStatusCodes.Status401Unauthorized));
            }

            if (order == null)
            {
                return new JsonResult(Return.Error("找不到此订单", StatusCodes.OrderNotFound));
            }

            if (order.paymentTime != null)
            {
                return new JsonResult(Return.Error("订单已被支付过", StatusCodes.OrderAlreadyPaid));
            }

            order.paymentTime = DateTime.Now;
            int result = orderDal.updateOrder(order);

            return new JsonResult(Return.Success(result));
        }

        // 关闭订单
        // POST /api/order/cancel
        [HttpPost("cancel")]
        public JsonResult cancelOrCloseOrder([FromBody]JObject data)
        {
            string sessionUserId = HttpContext.Session.GetString("userId");
            string sessionPassword = HttpContext.Session.GetString("password");

            UserDAL userDal = new UserDAL();
            OrderDAL orderDal = new OrderDAL();
            ItemDAL itemDal = new ItemDAL();
            ShopDAL shopDal = new ShopDAL();

            string orderId = data.Value<string>("order_id");
            Order order = orderDal.getOrderById(orderId);

            // 权限验证
            if (sessionUserId == null || userDal.getUserById(sessionUserId)?.password != sessionPassword ||
                order?.userId != sessionUserId)
            {
                return new JsonResult(Return.Error("用户未登录或无权限访问", HttpStatusCodes.Status401Unauthorized));
            }

            if (order == null)
            {
                return new JsonResult(Return.Error("找不到此订单", StatusCodes.OrderNotFound));
            }

            if (order.close == true)
            {
                return new JsonResult(Return.Error("订单已被关闭过", StatusCodes.OrderAlreadyClosed));
            }

            order.close = true;

            if (order.deliveryTime != null && order.paymentTime != null)
            {
                // 如果发货并关闭订单，认为是成交了
                order.receiptTime = DateTime.Now;

                // 增加销量
                foreach (OrderItem item in order.items)
                {
                    item.item.sales += item.count;
                    itemDal.updateItem(item.item);
                }

                // 增加用户积分
                User user = userDal.getUserById(sessionUserId);
                user.point += (int)(order.price * 100);
                user.level = (int)Math.Floor(0.005 * Math.Pow(user.point, 0.65));
                userDal.updateUserInfo(user);

                // 增加店铺积分
                foreach (OrderItem item in order.items)
                {
                    string shopId = item.item.shopId;
                    Shop shop = shopDal.getShopById(shopId);
                    shop.point += (int)(item.count * item.item.price * 100);
                    shop.level = (int)Math.Floor(0.005 * Math.Pow(shop.point, 0.65));
                    shopDal.updateShop(shop);
                }
            }
            else
            {
                // 否则认为是取消订单，增加库存量
                foreach (OrderItem item in order.items)
                {
                    item.item.quantity += item.count;
                    itemDal.updateItem(item.item);
                }
            }

            int result = orderDal.updateOrder(order);
            return new JsonResult(Return.Success(result));
        }
    }
}
