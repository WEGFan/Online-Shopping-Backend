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
    [Route("api/shoppingCart")]
    [ApiController]
    public class ShoppingCartsController : Controller
    {
        private readonly ILogger<ShoppingCartsController> logger;

        public ShoppingCartsController(ILogger<ShoppingCartsController> logger)
        {
            this.logger = logger;
        }

        private const int pageSize = 8;

        // 获取用户的购物车
        // GET /api/shoppingCart/list
        [HttpGet("list")]
        public JsonResult listShoppingCart()
        {
            // 登录验证
            string sessionUserId = HttpContext.Session.GetString("userId");
            string sessionPassword = HttpContext.Session.GetString("password");
            UserDAL userDal = new UserDAL();
            if (sessionUserId == null || userDal.getUserById(sessionUserId)?.password != sessionPassword)
            {
                return new JsonResult(Return.Error("用户未登录或无权限访问", HttpStatusCodes.Status401Unauthorized));
            }

            ShoppingCartItemDAL shoppingCartDal = new ShoppingCartItemDAL();
            List<ShoppingCartItem> result = shoppingCartDal.listShoppingCartItemsByUserId(sessionUserId);
            return new JsonResult(Return.Success(result));
        }

        // 创建购物车项目
        // POST /api/shoppingCart/add
        [HttpPost("add")]
        public JsonResult addShoppingCartItem([FromBody]ShoppingCartItem shoppingCartItem)
        {
            // 登录验证
            string sessionUserId = HttpContext.Session.GetString("userId");
            string sessionPassword = HttpContext.Session.GetString("password");
            UserDAL userDal = new UserDAL();
            if (sessionUserId == null || userDal.getUserById(sessionUserId)?.password != sessionPassword)
            {
                return new JsonResult(Return.Error("用户未登录或无权限访问", HttpStatusCodes.Status401Unauthorized));
            }

            shoppingCartItem.userId = sessionUserId;
            shoppingCartItem.createTime = DateTime.Now;

            // 模型验证
            ModelState.Clear();
            TryValidateModel(shoppingCartItem);
            if (!ModelState.IsValid)
            {
                return new JsonResult(Return.ModelError(ModelState));
            }

            ItemDAL itemDal = new ItemDAL();
            if (itemDal.getItemById(shoppingCartItem.itemId)?.open != true)
            {
                return new JsonResult(Return.Error("找不到此商品或商品已下架", StatusCodes.ItemNotFound));
            }

            shoppingCartItem.count = Math.Max(Math.Min(shoppingCartItem.count, 99), 1);

            ShoppingCartItemDAL shoppingCartItemDal = new ShoppingCartItemDAL();
            int result = shoppingCartItemDal.addShoppingCartItem(shoppingCartItem);
            return new JsonResult(Return.Success(result));
        }

        // 更新购物车项目
        // POST /api/shoppingCart/update
        [HttpPost("update")]
        public JsonResult updateShoppingCartItem([FromBody]JObject data)
        {
            string sessionUserId = HttpContext.Session.GetString("userId");
            string sessionPassword = HttpContext.Session.GetString("password");

            UserDAL userDal = new UserDAL();

            // 权限验证
            if (sessionUserId == null || userDal.getUserById(sessionUserId)?.password != sessionPassword)
            {
                return new JsonResult(Return.Error("用户未登录或无权限访问", HttpStatusCodes.Status401Unauthorized));
            }

            string itemId = data.Value<string>("item_id");

            ShoppingCartItemDAL shoppingCartItemDal = new ShoppingCartItemDAL();
            ShoppingCartItem shoppingCartItem = shoppingCartItemDal.getShoppingCartItemByUserIdAndItemId(sessionUserId, itemId);

            if (shoppingCartItem == null)
            {
                return new JsonResult(Return.Error("找不到此购物车商品", StatusCodes.ItemNotFound));
            }

            int? count = data.Value<int?>("count");
            if (count == null)
            {
                return new JsonResult(Return.Error("数量不能为空", HttpStatusCodes.Status400BadRequest));
            }

            shoppingCartItem.count = Math.Max(Math.Min((int)count, 99), 1);

            int result = shoppingCartItemDal.updateShoppingCartItem(shoppingCartItem);
            return new JsonResult(Return.Success(new { count = shoppingCartItem.count }));
        }

        // 删除购物车项目
        // POST /api/shoppingCart/remove
        [HttpPost("remove")]
        public JsonResult deleteShoppingCartItem([FromBody]JObject data)
        {
            string sessionUserId = HttpContext.Session.GetString("userId");
            string sessionPassword = HttpContext.Session.GetString("password");

            UserDAL userDal = new UserDAL();

            // 权限验证
            if (sessionUserId == null || userDal.getUserById(sessionUserId)?.password != sessionPassword)
            {
                return new JsonResult(Return.Error("用户未登录或无权限访问", HttpStatusCodes.Status401Unauthorized));
            }

            string itemId = data.Value<string>("item_id");

            ShoppingCartItemDAL shoppingCartItemDal = new ShoppingCartItemDAL();
            ShoppingCartItem shoppingCartItem = shoppingCartItemDal.getShoppingCartItemByUserIdAndItemId(sessionUserId, itemId);

            if (shoppingCartItem == null)
            {
                return new JsonResult(Return.Error("找不到此购物车商品", StatusCodes.ItemNotFound));
            }

            int result = shoppingCartItemDal.deleteShoppingCartItem(shoppingCartItem);
            return new JsonResult(Return.Success(result));
        }
    }
}
