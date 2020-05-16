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
    [Route("api/collection")]
    [ApiController]
    public class FavoriteItemsController : Controller
    {
        private readonly ILogger<FavoriteItemsController> logger;

        public FavoriteItemsController(ILogger<FavoriteItemsController> logger)
        {
            this.logger = logger;
        }

        private const int pageSize = 8;

        // 获取用户的收藏商品列表
        // GET /api/collection/list?page=[int]
        [HttpGet("list")]
        public JsonResult listFavoriteItems([FromQuery]int page = 1)
        {
            // 登录验证
            string sessionUserId = HttpContext.Session.GetString("userId");
            string sessionPassword = HttpContext.Session.GetString("password");
            UserDAL userDal = new UserDAL();
            if (sessionUserId == null || userDal.getUserById(sessionUserId)?.password != sessionPassword)
            {
                return new JsonResult(Return.Error("用户未登录或无权限访问", HttpStatusCodes.Status401Unauthorized));
            }

            FavoriteItemDAL favoriteItemDal = new FavoriteItemDAL();
            List<Item> result = favoriteItemDal.listFavoriteItemsByUserId(sessionUserId, page, pageSize);
            return new JsonResult(Return.Success(result));
        }

        // 增加或删除收藏商品项目
        // POST /api/collection/add
        [HttpPost("add")]
        public JsonResult addFavoriteItem([FromBody]JObject data)
        {
            // 登录验证
            string sessionUserId = HttpContext.Session.GetString("userId");
            string sessionPassword = HttpContext.Session.GetString("password");
            UserDAL userDal = new UserDAL();
            if (sessionUserId == null || userDal.getUserById(sessionUserId)?.password != sessionPassword)
            {
                return new JsonResult(Return.Error("用户未登录或无权限访问", HttpStatusCodes.Status401Unauthorized));
            }

            string itemId = data.Value<string>("item_id"); // 商品 ID

            if (itemId == null)
            {
                return new JsonResult(Return.Error("商品 ID 不能为空", HttpStatusCodes.Status400BadRequest));
            }

            ItemDAL itemDal = new ItemDAL();
            if (itemDal.getItemById(itemId) == null)
            {
                return new JsonResult(Return.Error("找不到此商品", StatusCodes.ItemNotFound));
            }

            FavoriteItemDAL favoriteItemDal = new FavoriteItemDAL();
            bool isFavoriteAdded = favoriteItemDal.toggleFavoriteItem(sessionUserId, itemId);

            var result = new { added = isFavoriteAdded }; // 返回是增加还是删除收藏项目
            return new JsonResult(Return.Success(result));
        }
    }
}
