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
    [Route("api/item")]
    [ApiController]
    public class ItemsController : Controller
    {
        private readonly ILogger<ItemsController> logger;

        public ItemsController(ILogger<ItemsController> logger)
        {
            this.logger = logger;
        }

        private const int pageSize = 18; // 每页显示数量

        // 获取商品列表
        // GET /api/item/list?detailed=[bool]&page=[int]&search=[string]&shopId=[string]&order=[string:(name|price|sales)]&ascending=[bool]
        [HttpGet("list")]
        public JsonResult listItems([FromQuery]bool detailed = false, [FromQuery]int page = 1, [FromQuery]string search = null,
                                        [FromQuery]string shop_id = null, [FromQuery]string order = null, [FromQuery]bool ascending = true)
        {
            ItemDAL itemDal = new ItemDAL();

            // 将搜索字符串分割成列表
            List<string> keywordList = search != null ? Regex.Split(search, @"\s+").ToList() : new List<string> { };

            if (!(new string[] { "name", "price", "sales" }).Contains(order))
            {
                order = "name";
            }

            List<Item> result = itemDal.listItems(true, keywordList, shop_id, order, ascending, page, pageSize);

            if (detailed)
            {
                return new JsonResult(Return.Success(result));
            }

            // 列表页面不显示商品描述、库存量、创建时间
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new LimitPropertiesContractResolver(new string[] { "description", "quantity", "create_time" }, false)
            };

            return new JsonResult(Return.Success(result), jsonSettings);
        }

        // 获取主页横幅商品列表
        // GET /api/item/bannerList
        [HttpGet("bannerList")]
        public JsonResult listBannerItems()
        {
            ItemDAL itemDal = new ItemDAL();
            List<Item> result = itemDal.randomChooseItems(10);

            // 列表页面不显示商品描述、库存量、创建时间
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new LimitPropertiesContractResolver(new string[] { "description", "quantity", "create_time" }, false)
            };

            return new JsonResult(Return.Success(result), jsonSettings);
        }

        // 获取商品 ID 为 {id} 的商品
        // GET /api/item/info?itemId={id}
        [HttpGet("info")]
        public JsonResult getItemInfo([FromQuery]string item_id)
        {
            string sessionUserId = HttpContext.Session.GetString("userId");

            ItemDAL itemDal = new ItemDAL();
            Item result = itemDal.getItemById(item_id, sessionUserId);

            if (result == null)
            {
                return new JsonResult(Return.Error("找不到此商品", StatusCodes.ItemNotFound));
            }
            return new JsonResult(Return.Success(result));
        }
    }
}
