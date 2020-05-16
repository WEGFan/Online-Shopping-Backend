using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SqlSugar;

namespace OnlineShoppingBackend.Models
{
    [SugarTable("order")]
    public class Order
    {
        /// <summary>
        /// 订单 ID
        /// </summary>
        [SugarColumn(ColumnName = "order_id", IsPrimaryKey = true)]
        public string orderId { get; set; }

        /// <summary>
        /// 用户 ID
        /// </summary>
        [SugarColumn(ColumnName = "user_id")]
        [Required(ErrorMessage = "用户 ID 不能为空")]
        public string userId { get; set; }

        /// <summary>
        /// 订单价格
        /// </summary>
        [Required(ErrorMessage = "订单价格")]
        public decimal price { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "create_time")]
        [Required(ErrorMessage = "创建时间不能为空")]
        public DateTime createTime { get; set; }

        /// <summary>
        /// 付款时间
        /// </summary>
        [SugarColumn(ColumnName = "payment_time")]
        public DateTime? paymentTime { get; set; }

        /// <summary>
        /// 发货时间
        /// </summary>
        [SugarColumn(ColumnName = "delivery_time")]
        public DateTime? deliveryTime { get; set; }

        /// <summary>
        /// 收货时间
        /// </summary>
        [SugarColumn(ColumnName = "receipt_time")]
        public DateTime? receiptTime { get; set; }

        /// <summary>
        /// 是否关闭
        /// </summary>
        [Required(ErrorMessage = "关闭状态不能为空")]
        public bool close { get; set; }

        /// <summary>
        /// 收货人姓名
        /// </summary>
        [SugarColumn(ColumnName = "accept_user_name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "收货人姓名不能为空")]
        public string acceptUserName { get; set; }

        /// <summary>
        /// 收货人地址
        /// </summary>
        [SugarColumn(ColumnName = "accept_user_address")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "收货人地址不能为空")]
        public string acceptUserAddress { get; set; }

        /// <summary>
        /// 收货人电话
        /// </summary>
        [SugarColumn(ColumnName = "accept_user_phone_number")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "收货人电话不能为空")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "手机号长度必须为 11 位")]
        public string acceptUserPhoneNumber { get; set; } // TODO: 正则表达式校验

        /// <summary>
        /// 备注
        /// </summary>
        public string mark { get; set; }

        /// <summary>
        /// 订单的商品
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [Required(ErrorMessage = "订单商品不能为空")]
        public List<OrderItem> items { get; set; }
    }
}
