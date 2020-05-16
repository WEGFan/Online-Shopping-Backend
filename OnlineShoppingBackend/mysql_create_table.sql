/*
 Source Server Type    : MySQL
 Source Server Version : 50719
 Source Schema         : online_shopping
 File Encoding         : 65001
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for item
-- ----------------------------
DROP TABLE IF EXISTS `item`;
CREATE TABLE `item`  (
  `item_id` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '商品ID',
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '名称',
  `description` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '描述',
  `price` decimal(10, 2) UNSIGNED NOT NULL DEFAULT 0.00 COMMENT '价格',
  `image_path` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '图片',
  `shop_id` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '所属店铺ID',
  `address` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '发货地',
  `sales` int(10) UNSIGNED NOT NULL DEFAULT 0 COMMENT '销量',
  `quantity` int(10) UNSIGNED NOT NULL DEFAULT 0 COMMENT '库存量',
  `create_time` datetime(0) NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建日期',
  `open` tinyint(1) UNSIGNED NOT NULL DEFAULT 0 COMMENT '是否上架 0否1是 默认0',
  PRIMARY KEY (`item_id`) USING BTREE,
  INDEX `index_item_name_price`(`name`, `price`) USING BTREE COMMENT '名称、价格组合索引',
  INDEX `index_item_name_sales`(`name`, `sales`) USING BTREE COMMENT '名称、销量组合索引',
  INDEX `index_item_name_price_sales`(`name`, `price`, `sales`) USING BTREE COMMENT '名称、价格、销量组合索引',
  INDEX `index_item_open`(`open`) USING BTREE COMMENT '上架状态索引',
  INDEX `fk_item_shop_shopid_shopid`(`shop_id`) USING BTREE,
  FULLTEXT INDEX `index_item_name`(`name`) COMMENT '商品名称索引',
  CONSTRAINT `fk_item_shop_shopid_shopid` FOREIGN KEY (`shop_id`) REFERENCES `shop` (`shop_id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for item_tag
-- ----------------------------
DROP TABLE IF EXISTS `item_tag`;
CREATE TABLE `item_tag`  (
  `item_id` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '商品ID',
  `tag_id` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '标签ID',
  UNIQUE INDEX `index_itemtag_unique`(`item_id`, `tag_id`) USING BTREE COMMENT '唯一索引',
  INDEX `index_itemtag_itemid`(`item_id`) USING BTREE COMMENT '商品ID索引',
  INDEX `index_itemtag_tagid`(`tag_id`) USING BTREE COMMENT '标签ID索引',
  CONSTRAINT `fk_itemtag_item` FOREIGN KEY (`item_id`) REFERENCES `item` (`item_id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_itemtag_tag` FOREIGN KEY (`tag_id`) REFERENCES `tag` (`tag_id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for order
-- ----------------------------
DROP TABLE IF EXISTS `order`;
CREATE TABLE `order`  (
  `order_id` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '订单ID',
  `user_id` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '用户ID',
  `price` decimal(10, 2) NOT NULL COMMENT '订单价格',
  `create_time` datetime(0) NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `payment_time` datetime(0) NULL DEFAULT NULL COMMENT '付款时间',
  `delivery_time` datetime(0) NULL DEFAULT NULL COMMENT '发货时间',
  `receipt_time` datetime(0) NULL DEFAULT NULL COMMENT '收货时间',
  `close` tinyint(1) UNSIGNED NOT NULL DEFAULT 0 COMMENT '是否关闭 0否1是 默认0',
  `accept_user_name` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '收货人姓名',
  `accept_user_address` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '收货人地址',
  `accept_user_phone_number` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '收货人电话',
  `mark` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`order_id`) USING BTREE,
  INDEX `index_order_userid`(`user_id`) USING BTREE COMMENT '用户ID索引',
  INDEX `index_order_close`(`close`) USING BTREE COMMENT '开启关闭状态索引',
  CONSTRAINT `fk_order_user_userid` FOREIGN KEY (`user_id`) REFERENCES `user` (`user_id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for order_item
-- ----------------------------
DROP TABLE IF EXISTS `order_item`;
CREATE TABLE `order_item`  (
  `order_id` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '订单ID',
  `item_id` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '商品ID',
  `count` int(10) UNSIGNED NOT NULL COMMENT '数量',
  PRIMARY KEY (`order_id`, `item_id`) USING BTREE,
  INDEX `fk_orderitem_item_itemid`(`item_id`) USING BTREE,
  CONSTRAINT `fk_orderitem_item_itemid` FOREIGN KEY (`item_id`) REFERENCES `item` (`item_id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_orderitem_order_orderid` FOREIGN KEY (`order_id`) REFERENCES `order` (`order_id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for shop
-- ----------------------------
DROP TABLE IF EXISTS `shop`;
CREATE TABLE `shop`  (
  `shop_id` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '店铺ID',
  `name` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '名称',
  `level` int(10) UNSIGNED NOT NULL DEFAULT 0 COMMENT '等级',
  `point` int(10) UNSIGNED NOT NULL DEFAULT 0 COMMENT '积分（用来计算级别',
  `followers` int(10) UNSIGNED NOT NULL DEFAULT 0 COMMENT '粉丝',
  `open` tinyint(2) UNSIGNED NOT NULL DEFAULT 0 COMMENT '是否开启 0否1是 默认0',
  `create_time` datetime(0) NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  PRIMARY KEY (`shop_id`) USING BTREE,
  FULLTEXT INDEX `index_shop_name`(`name`) COMMENT '店铺名称索引'
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for tag
-- ----------------------------
DROP TABLE IF EXISTS `tag`;
CREATE TABLE `tag`  (
  `tag_id` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '标签的ID',
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '名称',
  PRIMARY KEY (`tag_id`) USING BTREE,
  INDEX `index_tag_tagid`(`tag_id`) USING BTREE COMMENT '标签ID的索引',
  FULLTEXT INDEX `index_tag_name`(`name`) COMMENT '标签名称索引'
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for user
-- ----------------------------
DROP TABLE IF EXISTS `user`;
CREATE TABLE `user`  (
  `user_id` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '用户ID',
  `account` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '账号',
  `password` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '加盐后密码',
  `salt` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '盐分',
  `email` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '邮箱',
  `phone_number` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '手机号',
  `nickname` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '昵称',
  `level` int(10) UNSIGNED NOT NULL DEFAULT 0 COMMENT '等级',
  `point` int(10) UNSIGNED NOT NULL DEFAULT 0 COMMENT '积分（计算等级',
  `banned` tinyint(2) UNSIGNED NOT NULL DEFAULT 0 COMMENT '账号状态',
  `shop_owner` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '店铺管理者',
  PRIMARY KEY (`user_id`) USING BTREE,
  UNIQUE INDEX `index_user_account`(`account`) USING BTREE COMMENT '帐号唯一索引',
  INDEX `index_user_nickname`(`nickname`) USING BTREE COMMENT '昵称索引',
  INDEX `fk_user_shopowner`(`shop_owner`) USING BTREE,
  INDEX `index_user_userid`(`user_id`) USING BTREE COMMENT '用户ID索引',
  CONSTRAINT `fk_user_shopowner` FOREIGN KEY (`shop_owner`) REFERENCES `shop` (`shop_id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for user_favorite_item
-- ----------------------------
DROP TABLE IF EXISTS `user_favorite_item`;
CREATE TABLE `user_favorite_item`  (
  `user_id` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '用户ID',
  `item_id` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '商品ID',
  UNIQUE INDEX `index_userfavoriteitem_unique`(`user_id`, `item_id`) USING BTREE COMMENT '唯一索引',
  INDEX `index_userfavoriteitem_userid`(`user_id`) USING BTREE COMMENT '用户ID索引',
  INDEX `index_userfavoriteitem_itemid`(`item_id`) USING BTREE COMMENT '商品ID索引',
  CONSTRAINT `fk_useritemcollection_item` FOREIGN KEY (`item_id`) REFERENCES `item` (`item_id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_useritemcollection_user` FOREIGN KEY (`user_id`) REFERENCES `user` (`user_id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for user_shopping_cart
-- ----------------------------
DROP TABLE IF EXISTS `user_shopping_cart`;
CREATE TABLE `user_shopping_cart`  (
  `user_id` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '用户ID',
  `item_id` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '商品ID',
  `count` int(10) UNSIGNED NOT NULL DEFAULT 1 COMMENT '数量',
  `create_time` datetime(0) NOT NULL ON UPDATE CURRENT_TIMESTAMP(0) COMMENT '创建时间',
  PRIMARY KEY (`user_id`, `item_id`) USING BTREE,
  UNIQUE INDEX `index_useritemshoppingcart_unique`(`user_id`, `item_id`) USING BTREE COMMENT '唯一索引',
  INDEX `index_useritemshoppingcart_userid`(`user_id`) USING BTREE COMMENT '用户ID索引',
  INDEX `index_useritemshoppingcart_itemid`(`item_id`) USING BTREE COMMENT '商品ID索引',
  CONSTRAINT `fk_useritemshoppingcart_user` FOREIGN KEY (`user_id`) REFERENCES `user` (`user_id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fkuseritemshoppingcart_item` FOREIGN KEY (`item_id`) REFERENCES `item` (`item_id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;
