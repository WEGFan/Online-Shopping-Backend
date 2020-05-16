using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace OnlineShoppingBackend.Utils
{
    public class Return
    {
        public class ReturnJson
        {
            public int code { get; set; }
            public string message { get; set; }
            public object data { get; set; }
        }

        public static ReturnJson ComJson(string responseMessage, int responseCode, object rtnData = null)
        {
            ReturnJson returnJson = new ReturnJson()
            {
                code = responseCode,
                message = responseMessage,
                data = rtnData ?? new object { }
            };
            return returnJson;
        }

        public static ReturnJson Success(object rtnData = null)
        {
            ReturnJson returnJson = new ReturnJson()
            {
                code = 200,
                message = string.Empty,
                data = rtnData ?? new object { }
            };
            return returnJson;
        }

        public static ReturnJson Error(string responseMessage, int responseCode = 200, object rtnData = null)
        {
            ReturnJson returnJson = new ReturnJson()
            {
                code = responseCode,
                message = responseMessage ?? string.Empty,
                data = rtnData ?? new object { }
            };
            return returnJson;
        }

        public static ReturnJson ModelError(ModelStateDictionary modelState, int responseCode = 200, object rtnData = null)
        {
            // 拼接所有错误信息
            string messages = string.Join("; ", modelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));

            ReturnJson returnJson = new ReturnJson()
            {
                code = responseCode,
                message = messages,
                data = rtnData ?? new object { }
            };
            return returnJson;
        }
    }
}
