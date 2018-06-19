-- MySQL dump 10.13  Distrib 5.6.19, for Win64 (x86_64)
--
-- Host: 113.98.255.71    Database: admin_center
-- ------------------------------------------------------
-- Server version	5.7.3-m13

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `admin_group`
--

DROP TABLE IF EXISTS `admin_group`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `admin_group` (
  `AdminGroupId` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(45) NOT NULL DEFAULT '',
  `Type` int(11) NOT NULL DEFAULT '1' COMMENT '0-超级管理员（默认拥有所有权限）1-普通管理员（设定其拥有的权限）',
  `Description` varchar(200) DEFAULT NULL,
  `Disabled` bit(1) NOT NULL DEFAULT b'0',
  `Deleted` bit(1) NOT NULL DEFAULT b'0',
  `CreateBy` varchar(45) NOT NULL DEFAULT '',
  `CreateOn` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '必须为UTC时间',
  `UpdateBy` varchar(45) DEFAULT NULL,
  `UpdateOn` datetime DEFAULT NULL COMMENT '必须为UTC时间',
  PRIMARY KEY (`AdminGroupId`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `admin_group_permission`
--

DROP TABLE IF EXISTS `admin_group_permission`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `admin_group_permission` (
  `AdminGroupPermissionId` int(11) NOT NULL AUTO_INCREMENT,
  `GroupId` int(11) NOT NULL DEFAULT '0',
  `PermissionId` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`AdminGroupPermissionId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `admin_permission`
--

DROP TABLE IF EXISTS `admin_permission`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `admin_permission` (
  `AdminPermissionId` int(11) NOT NULL AUTO_INCREMENT,
  `SystemId` int(11) NOT NULL DEFAULT '0',
  `ParentId` int(11) NOT NULL DEFAULT '-1',
  `PermissionCode` varchar(100) NOT NULL DEFAULT '',
  `Name` varchar(45) NOT NULL DEFAULT '',
  `Img` varchar(200) DEFAULT NULL COMMENT '根权限图标（即可为样式名称，也可为IMG路径，视具体情况而定）',
  `Description` varchar(45) DEFAULT NULL,
  `IsMenu` bit(1) NOT NULL DEFAULT b'0',
  `IsLink` bit(1) NOT NULL DEFAULT b'0',
  `Url` varchar(200) DEFAULT NULL,
  `Target` varchar(45) DEFAULT NULL,
  `SortNo` int(11) NOT NULL DEFAULT '1',
  `Disabled` bit(1) NOT NULL DEFAULT b'0',
  `Deleted` bit(1) NOT NULL DEFAULT b'0',
  `CreateBy` varchar(45) NOT NULL DEFAULT '',
  `CreateOn` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '必须为UTC时间',
  `UpdateBy` varchar(45) DEFAULT NULL,
  `UpdateOn` datetime DEFAULT NULL COMMENT '必须为UTC时间',
  PRIMARY KEY (`AdminPermissionId`)
) ENGINE=InnoDB AUTO_INCREMENT=72 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `admin_system`
--

DROP TABLE IF EXISTS `admin_system`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `admin_system` (
  `AdminSystemId` int(11) NOT NULL AUTO_INCREMENT,
  `SysKey` varchar(45) NOT NULL DEFAULT '',
  `Code` varchar(45) NOT NULL DEFAULT '' COMMENT '添加系统对应的权限时起辅助作用（该Code值将默认作为权限Code的前缀，此值允许用户添加权限时自定义修改）',
  `Name` varchar(45) NOT NULL DEFAULT '',
  `Description` varchar(200) DEFAULT NULL,
  `URL` varchar(200) NOT NULL DEFAULT '',
  `CallBackUrl` varchar(200) NOT NULL DEFAULT '',
  `Logo` varchar(200) DEFAULT NULL,
  `Deleted` bit(1) NOT NULL DEFAULT b'0',
  `CreateBy` varchar(45) NOT NULL DEFAULT '',
  `CreateOn` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '必须为UTC时间',
  `UpdateBy` varchar(45) DEFAULT NULL,
  `UpdateOn` datetime DEFAULT NULL COMMENT '必须为UTC时间',
  PRIMARY KEY (`AdminSystemId`)
) ENGINE=InnoDB AUTO_INCREMENT=41 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `admin_user`
--

DROP TABLE IF EXISTS `admin_user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `admin_user` (
  `AdminUserId` int(11) NOT NULL AUTO_INCREMENT,
  `AdminName` varchar(45) NOT NULL DEFAULT '',
  `Password` varchar(45) NOT NULL DEFAULT '',
  `RealName` varchar(45) NOT NULL DEFAULT '',
  `Logo` varchar(200) DEFAULT NULL,
  `Disabled` bit(1) NOT NULL DEFAULT b'0',
  `Deleted` bit(1) NOT NULL DEFAULT b'0',
  `CreateBy` varchar(45) NOT NULL DEFAULT '',
  `CreateOn` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '必须为UTC时间',
  `UpdateBy` varchar(45) DEFAULT NULL,
  `UpdateOn` datetime DEFAULT NULL COMMENT '必须为UTC时间',
  PRIMARY KEY (`AdminUserId`)
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `admin_user_group`
--

DROP TABLE IF EXISTS `admin_user_group`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `admin_user_group` (
  `AdminUserGroupId` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` int(11) NOT NULL DEFAULT '0',
  `GroupId` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`AdminUserGroupId`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `admin_user_permission`
--

DROP TABLE IF EXISTS `admin_user_permission`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `admin_user_permission` (
  `AdminUserPermissionId` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` int(11) NOT NULL DEFAULT '0',
  `PermissionId` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`AdminUserPermissionId`)
) ENGINE=InnoDB AUTO_INCREMENT=198 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2014-07-26 23:12:29
