using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SqlSugar;

namespace OnlineShoppingBackend.Models
{
    [SugarTable("tag")]
    public class Tag
    {
        /// <summary>
        /// 标签 ID
        /// </summary>
        [SugarColumn(ColumnName = "tag_id", IsPrimaryKey = true)]
        public string tagId { get; set; }

        /// <summary>
        /// 标签名
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "标签名不能为空")]
        public string name { get; set; }
    }
}
