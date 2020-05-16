using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShoppingBackend.Utils
{
    public class StatusCodes
    {
        /// <summary>
        /// 用户名重复
        /// </summary>
        public const int DuplicateUser = 50000;

        /// <summary>
        /// 用户名或密码错误
        /// </summary>
        public const int WrongUsernameOrPassword = 50001;

        /// <summary>
        /// 找不到用户
        /// </summary>
        public const int UserNotFound = 50002;

        /// <summary>
        /// 找不到商品
        /// </summary>
        public const int ItemNotFound = 50003;

        /// <summary>
        /// 找不到店铺
        /// </summary>
        public const int ShopNotFound = 50004;

        /// <summary>
        /// 找不到订单
        /// </summary>
        public const int OrderNotFound = 50005;

        /// <summary>
        /// 库存量不足
        /// </summary>
        public const int NotEnoughItems = 50006;

        /// <summary>
        /// 订单已被关闭
        /// </summary>
        public const int OrderAlreadyClosed = 50007;

        /// <summary>
        /// 订单已被支付
        /// </summary>
        public const int OrderAlreadyPaid = 50008;
    }
}
