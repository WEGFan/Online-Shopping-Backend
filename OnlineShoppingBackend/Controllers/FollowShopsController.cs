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
    [Route("api/follow")]
    [ApiController]
    public class FollowShopsController : Controller
    {
        private readonly ILogger<FollowShopsController> logger;

        public FollowShopsController(ILogger<FollowShopsController> logger)
        {
            this.logger = logger;
        }

        private const int pageSize = 8;

        // 获取用户的关注店铺列表
        // GET /api/follow/list?page=[int]
        [HttpGet("list")]
        public JsonResult listFollowedShops([FromQuery]int page = 1)
        {
            // 登录验证
            string sessionUserId = HttpContext.Session.GetString("userId");
            string sessionPassword = HttpContext.Session.GetString("password");
            UserDAL userDal = new UserDAL();
            if (sessionUserId == null || userDal.getUserById(sessionUserId)?.password != sessionPassword)
            {
                return new JsonResult(Return.Error("用户未登录或无权限访问", HttpStatusCodes.Status401Unauthorized));
            }

            FollowShopDAL followShopDal = new FollowShopDAL();
            List<Shop> result = followShopDal.listFollowShopsByUserId(sessionUserId, page, pageSize);
            return new JsonResult(Return.Success(result));
        }

        // 切换关注店铺项目
        // POST /api/follow/add
        [HttpPost("add")]
        public JsonResult addFollowedShop([FromBody]JObject data)
        {
            // 登录验证
            string sessionUserId = HttpContext.Session.GetString("userId");
            string sessionPassword = HttpContext.Session.GetString("password");
            UserDAL userDal = new UserDAL();
            if (sessionUserId == null || userDal.getUserById(sessionUserId)?.password != sessionPassword)
            {
                return new JsonResult(Return.Error("用户未登录或无权限访问", HttpStatusCodes.Status401Unauthorized));
            }

            string shopId = data.Value<string>("shop_id"); // 店铺 ID
            if (shopId == null)
            {
                return new JsonResult(Return.Error("店铺 ID 不能为空", HttpStatusCodes.Status400BadRequest));
            }

            ShopDAL shopDal = new ShopDAL();
            Shop shop = shopDal.getShopById(shopId);
            if (shop == null)
            {
                return new JsonResult(Return.Error("找不到此店铺", StatusCodes.ShopNotFound));
            }

            FollowShopDAL followShopDal = new FollowShopDAL();
            bool isFollowAdded = followShopDal.toggleFollowShop(sessionUserId, shopId);

            // 修改店铺的粉丝数
            if (isFollowAdded)
            {
                shop.followers++;
            }
            else
            {
                shop.followers--;
            }
            shopDal.updateShop(shop);

            var result = new { added = isFollowAdded };
            return new JsonResult(Return.Success(result));
        }
    }
}
