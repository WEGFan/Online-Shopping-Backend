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
    [Route("api/shop")]
    [ApiController]
    public class ShopsController : Controller
    {
        private readonly ILogger<ShopsController> logger;

        public ShopsController(ILogger<ShopsController> logger)
        {
            this.logger = logger;
        }

        private const int pageSize = 18; // 每页显示数量

        // 获取店铺
        // GET /api/shop/list?page=[int]&search=[string]&order=[string:(name|point|followers)]&ascending=[bool]
        [HttpGet("list")]
        public JsonResult listShops([FromQuery]int page = 1, [FromQuery]string search = null,
                                        [FromQuery]string order = null, [FromQuery]bool ascending = true)
        {
            ShopDAL shopDal = new ShopDAL();

            // 将搜索字符串分割成列表
            List<string> keywordList = search != null ? Regex.Split(search, @"\s+").ToList() : new List<string> { };

            if (!(new string[] { "name", "point", "followers" }).Contains(order))
            {
                order = "name";
            }

            List<Shop> result = shopDal.listShops(true, keywordList, order, ascending, page, pageSize);
            return new JsonResult(Return.Success(result));
        }

        // 获取店铺 ID 为 {id} 的店铺
        // GET /api/shop/info?shopId={id}
        [HttpGet("info")]
        public JsonResult getShopInfo([FromQuery]string shop_id)
        {
            ShopDAL shopDal = new ShopDAL();
            Shop result = shopDal.getShopById(shop_id);

            if (result == null)
            {
                return new JsonResult(Return.Error("找不到此店铺", StatusCodes.ShopNotFound));
            }

            return new JsonResult(Return.Success(result));
        }
    }
}
