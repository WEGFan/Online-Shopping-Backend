using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SqlSugar;

namespace OnlineShoppingBackend.Models
{
    [SugarTable("item")]
    public class Item
    {
        /// <summary>
        /// 商品 ID
        /// </summary>
        [SugarColumn(ColumnName = "item_id", IsPrimaryKey = true)]
        public string itemId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "名称不能为空")]
        public string name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Required(AllowEmptyStrings = true, ErrorMessage = "描述不能为空")]
        public string description { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        [Required(ErrorMessage = "价格不能为空")]
        public decimal price { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        [SugarColumn(ColumnName = "image_path")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "图片路径不能为空")]
        public string imagePath { get; set; }

        /// <summary>
        /// 所属店铺ID
        /// </summary>
        [SugarColumn(ColumnName = "shop_id")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "所属店铺ID不能为空")]
        public string shopId { get; set; }

        /// <summary>
        /// 发货地
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "发货地不能为空")]
        public string address { get; set; }

        /// <summary>
        /// 销量
        /// </summary>
        [Required(ErrorMessage = "销量不能为空")]
        public int sales { get; set; }

        /// <summary>
        /// 库存量
        /// </summary>
        [Required(ErrorMessage = "库存量不能为空")]
        public int quantity { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        [SugarColumn(ColumnName = "create_time")]
        [Required(ErrorMessage = "创建日期不能为空")]
        public DateTime createTime { get; set; }

        /// <summary>
        /// 是否上架
        /// </summary>
        [Required(ErrorMessage = "上架状态不能为空")]
        public bool open { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<string> tags { get; set; }

        /// <summary>
        /// 是否收藏
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public bool collected { get; set; }
    }
}
