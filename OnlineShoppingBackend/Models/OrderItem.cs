using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SqlSugar;

namespace OnlineShoppingBackend.Models
{
    [SugarTable("order_item")]
    public class OrderItem
    {
        /// <summary>
        /// 订单 ID
        /// </summary>
        [SugarColumn(ColumnName = "order_id")]
        [JsonIgnore]
        public string orderId { get; set; }

        /// <summary>
        /// 商品 ID
        /// </summary>
        [SugarColumn(ColumnName = "item_id")]
        public string itemId { get; set; }

        /// <summary>
        /// 商品
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public Item item { get; set; }

        /// <summary>
        /// 商品数量
        /// </summary>
        public int count { get; set; }
    }
}
