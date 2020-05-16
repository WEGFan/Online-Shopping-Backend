using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    [Route("api/user")]
    [ApiController]
    public class SessionController : Controller
    {
        private readonly ILogger<SessionController> logger;

        public SessionController(ILogger<SessionController> logger)
        {
            this.logger = logger;
        }

        // 检查用户是否登录
        // GET /api/user/hasLogin
        [HttpGet("hasLogin")]
        public JsonResult userHasLogin()
        {
            string userId = HttpContext.Session.GetString("userId");
            string password = HttpContext.Session.GetString("password");

            UserDAL userDal = new UserDAL();
            User user = userDal.getUserById(userId);

            if (userId == null || password == null || user.password != password)
            {
                return new JsonResult(Return.Success(new
                {
                    login = false
                }));
            }

            return new JsonResult(Return.Success(new
            {
                login = true
            }));
        }

        // 用户登录
        // POST /api/user/login
        [HttpPost("login")]
        public JsonResult userLogin([FromBody]JObject data)
        {
            string account = data.Value<string>("account");
            string password = data.Value<string>("password");

            if (account == null || password == null)
            {
                return new JsonResult(Return.Error("请填写用户名或密码", StatusCodes.WrongUsernameOrPassword));
            }

            UserDAL userDal = new UserDAL();
            User user = userDal.getUserByAccount(account);

            // 密码加盐
            PasswordEncryptor encryptor = new PasswordEncryptor();
            string saltPassword = encryptor.encryptPassword(password, user?.salt);

            if (user == null || saltPassword != user.password)
            {
                return new JsonResult(Return.Error("用户名或密码错误", StatusCodes.WrongUsernameOrPassword));
            }

            HttpContext.Session.SetString("userId", user.userId);
            HttpContext.Session.SetString("password", user.password);

            // 不显示敏感信息
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new LimitPropertiesContractResolver(new string[] { "password", "salt" }, false)
            };

            return new JsonResult(Return.Success(user), jsonSettings);
        }

        // 用户注销
        // POST /api/user/logout
        [HttpPost("logout")]
        public JsonResult userLogout()
        {
            HttpContext.Session.Clear();
            return new JsonResult(Return.Success());
        }
    }
}
