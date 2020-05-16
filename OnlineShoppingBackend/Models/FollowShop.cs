using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SqlSugar;

namespace OnlineShoppingBackend.Models
{
    [SugarTable("user_follow_shop")]
    public class FollowShop
    {
        /// <summary>
        /// 用户 ID
        /// </summary>
        [SugarColumn(ColumnName = "user_id")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "用户 ID 不能为空")]
        public string userId { get; set; }

        /// <summary>
        /// 店铺 ID
        /// </summary>
        [SugarColumn(ColumnName = "shop_id")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "店铺 ID 不能为空")]
        public string shopId { get; set; }

        /// <summary>
        /// 店铺
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public Shop shop { get; set; }
    }
}
