using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SqlSugar;

namespace OnlineShoppingBackend.Models
{
    [SugarTable("item_tag")]
    public class ItemTag
    {
        /// <summary>
        /// 商品 ID
        /// </summary>
        [SugarColumn(ColumnName = "item_id")]
        public string itemId { get; set; }

        /// <summary>
        /// 标签 ID
        /// </summary>
        [SugarColumn(ColumnName = "tag_id")]
        public string tagId { get; set; }
    }
}
