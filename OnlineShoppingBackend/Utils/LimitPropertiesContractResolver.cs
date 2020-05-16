using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OnlineShoppingBackend.Utils
{
    // https://www.cnblogs.com/PatrickLiu/p/10148573.html
    public class LimitPropertiesContractResolver : SnakeCaseNamingContractResolver
    {
        private string[] property;

        private bool isIncluded;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="property">属性数组</param>
        /// <param name="isIncluded">是否为要保留的字段</param>
        public LimitPropertiesContractResolver(string[] property, bool isIncluded)
        {
            this.property = property;
            this.isIncluded = isIncluded;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> list = base.CreateProperties(type, memberSerialization);

            // 只保留清单有列出的属性
            return list.Where(p => isIncluded ? property.Contains(p.PropertyName) : !property.Contains(p.PropertyName))
                        .ToList();
        }
    }
}