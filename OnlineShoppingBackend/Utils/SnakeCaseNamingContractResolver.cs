using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace OnlineShoppingBackend.Utils
{
    public class SnakeCaseNamingContractResolver : DefaultContractResolver
    {
        public SnakeCaseNamingContractResolver()
        {
            // 使用下划线命名方式
            NamingStrategy = new SnakeCaseNamingStrategy { ProcessDictionaryKeys = true };
        }
    }
}
