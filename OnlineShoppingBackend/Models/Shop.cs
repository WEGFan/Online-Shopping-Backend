using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SqlSugar;

namespace OnlineShoppingBackend.Models
{
    [SugarTable("shop")]
    public class Shop
    {
        /// <summary>
        /// 店铺 ID
        /// </summary>
        [SugarColumn(ColumnName = "shop_id", IsPrimaryKey = true)]
        public string shopId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "名称不能为空")]
        public string name { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        [Required(ErrorMessage = "等级不能为空")]
        public int level { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        [Required(ErrorMessage = "积分不能为空")]
        public int point { get; set; }

        /// <summary>
        /// 粉丝数
        /// </summary>
        [Required(ErrorMessage = "粉丝数不能为空")]
        public int followers { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Required(ErrorMessage = "创建时间不能为空")]
        [SugarColumn(ColumnName = "create_time")]
        public DateTime createTime { get; set; }

        /// <summary>
        /// 店铺是否开启
        /// </summary>
        [Required(ErrorMessage = "店铺开启状态不能为空")]
        public bool open { get; set; }
    }
}
