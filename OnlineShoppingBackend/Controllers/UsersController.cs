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
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> logger;

        public UsersController(ILogger<UsersController> logger)
        {
            this.logger = logger;
        }

        // 获取当前登录用户信息
        // GET /api/user/info
        [HttpGet("info")]
        public JsonResult getCurrentUserInfo()
        {
            // 登录验证
            string sessionUserId = HttpContext.Session.GetString("userId");
            string sessionPassword = HttpContext.Session.GetString("password");
            if (sessionUserId == null || sessionPassword == null)
            {
                return new JsonResult(Return.Error("用户未登录或无权限访问", HttpStatusCodes.Status401Unauthorized));
            }

            UserDAL userDal = new UserDAL();
            User result = userDal.getUserById(sessionUserId);

            // 不显示敏感信息
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new LimitPropertiesContractResolver(new string[] { "password", "salt" }, false)
            };

            return new JsonResult(Return.Success(result), jsonSettings);
        }

        // 注册用户
        // POST /api/user/register
        [HttpPost("register")]
        public JsonResult userRegister([FromBody]User user)
        {
            user.userId = Guid.NewGuid().ToString();
            user.level = user.point = 0;
            user.banned = false;

            // 模型验证
            ModelState.Clear();
            TryValidateModel(user);
            if (!ModelState.IsValid)
            {
                return new JsonResult(Return.ModelError(ModelState));
            }

            // 密码加盐
            PasswordEncryptor encryptor = new PasswordEncryptor();
            user.salt = encryptor.generateSalt();
            user.password = encryptor.encryptPassword(user.password, user.salt);

            UserDAL userDal = new UserDAL();
            if (userDal.checkAccountExists(user.account))
            {
                return new JsonResult(Return.Error("用户名重复", StatusCodes.DuplicateUser));
            }

            int result = userDal.addUser(user);

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new LimitPropertiesContractResolver(new string[] { "password", "salt" }, false)
            };

            return new JsonResult(Return.Success(user), jsonSettings);
        }

        // 更新用户信息
        // POST /api/user/updateInfo
        [HttpPost("updateInfo")]
        public JsonResult updateUserInfo([FromBody]User user)
        {
            UserDAL userDal = new UserDAL();

            string sessionUserId = HttpContext.Session.GetString("userId");
            string sessionPassword = HttpContext.Session.GetString("password");
            User currentUser = userDal.getUserById(sessionUserId);
            if (sessionUserId == null || currentUser.password != sessionPassword)
            {
                return new JsonResult(Return.Error("用户未登录或无权限访问", HttpStatusCodes.Status401Unauthorized));
            }

            user.userId = sessionUserId;
            user.account = currentUser.account;
            user.password = currentUser.password;
            user.salt = currentUser.salt;
            user.level = currentUser.level;
            user.point = currentUser.point;
            user.banned = currentUser.banned;
            user.shopOwner = currentUser.shopOwner;

            // 模型验证
            ModelState.Clear();
            TryValidateModel(user);
            ModelState.ClearValidationState("password"); // 不验证密码
            ModelState.MarkFieldValid("password");
            if (!ModelState.IsValid)
            {
                return new JsonResult(Return.ModelError(ModelState));
            }

            int result = userDal.updateUserInfo(user);
            return new JsonResult(Return.Success(result));
        }

        // 修改密码
        // POST /api/user/changePassword
        [HttpPost("changePassword")]
        public JsonResult changePassword([FromBody]JObject data)
        {
            UserDAL userDal = new UserDAL();

            string sessionUserId = HttpContext.Session.GetString("userId");
            string sessionPassword = HttpContext.Session.GetString("password");
            User user = userDal.getUserById(sessionUserId);
            if (sessionUserId == null || user.password != sessionPassword)
            {
                return new JsonResult(Return.Error("用户未登录或无权限访问", HttpStatusCodes.Status401Unauthorized));
            }

            string oldPassword = data.Value<string>("oldPassword"), // 旧密码
                newPassword = data.Value<string>("newPassword"); // 新密码

            // 密码加盐
            PasswordEncryptor encryptor = new PasswordEncryptor();
            string saltOldPassword = encryptor.encryptPassword(oldPassword, user?.salt);

            if (saltOldPassword != user.password)
            {
                return new JsonResult(Return.Error("原密码错误", HttpStatusCodes.Status400BadRequest));
            }

            if (newPassword == null)
            {
                return new JsonResult(Return.Error("新密码不能为空", HttpStatusCodes.Status400BadRequest));
            }

            if (newPassword.Length < 6 || newPassword.Length > 32)
            {
                return new JsonResult(Return.Error("新密码长度必须在 6 至 32 位之间", HttpStatusCodes.Status400BadRequest));
            }

            // 新密码加盐
            string saltNewPassword = encryptor.encryptPassword(newPassword, user?.salt);
            user.password = saltNewPassword;

            // 更改 session 密码值
            HttpContext.Session.SetString("password", saltNewPassword);

            int result = userDal.updateUserPassword(user);
            return new JsonResult(Return.Success(result));
        }
    }
}
