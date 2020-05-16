using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SqlSugar;

namespace OnlineShoppingBackend.Models
{
    [SugarTable("user")]
    public class User
    {
        /// <summary>
        /// 用户 ID
        /// </summary>
        [SugarColumn(ColumnName = "user_id", IsPrimaryKey = true)]
        public string userId { get; set; }

        /// <summary>
        /// 帐号
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "帐号不能为空")]
        [StringLength(32, MinimumLength = 6, ErrorMessage = "帐号长度必须在 6 至 32 位之间")]
        public string account { get; set; }

        /// <summary>
        /// 加盐后的密码
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "密码不能为空")]
        [StringLength(32, MinimumLength = 6, ErrorMessage = "密码长度必须在 6 至 32 位之间")]
        public string password { get; set; }

        /// <summary>
        /// 盐分
        /// </summary>
        public string salt { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "邮箱不能为空")]
        [RegularExpression(@"^([a-zA-Z]|[0-9])(\w|\-)+@[a-zA-Z0-9]+\.([a-zA-Z]{2,4})$", ErrorMessage = "邮箱格式不正确")]
        public string email { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [SugarColumn(ColumnName = "phone_number")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "手机号不能为空")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "手机号长度必须为 11 位")]
        [RegularExpression(@"^1[3456789]\d{9}$", ErrorMessage = "手机号格式不正确")]
        public string phoneNumber { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "昵称不能为空")]
        public string nickname { get; set; }

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
        /// 是否被封禁
        /// </summary>
        [Required(ErrorMessage = "帐号状态不能为空")]
        public bool banned { get; set; }

        /// <summary>
        /// 管理的店铺 ID
        /// </summary>
        [SugarColumn(ColumnName = "shop_owner")]
        public string shopOwner { get; set; }
    }
}
