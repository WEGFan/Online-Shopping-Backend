using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SqlSugar;

namespace OnlineShoppingBackend.Models
{
    [SugarTable("user_shopping_cart")]
    public class ShoppingCartItem
    {
        /// <summary>
        /// 用户 ID
        /// </summary>
        [SugarColumn(ColumnName = "user_id")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "用户 ID 不能为空")]
        public string userId { get; set; }

        /// <summary>
        /// 商品 ID
        /// </summary>
        [SugarColumn(ColumnName = "item_id")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "商品 ID 不能为空")]
        public string itemId { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [Required(ErrorMessage = "数量不能为空")]
        public int count { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "create_time")]
        public DateTime createTime { get; set; }

        /// <summary>
        /// 商品
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public Item item { get; set; }
    }
}
