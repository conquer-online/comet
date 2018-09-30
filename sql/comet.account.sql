CREATE DATABASE  IF NOT EXISTS `comet.account` /*!40100 DEFAULT CHARACTER SET utf8 COLLATE utf8_bin */;
USE `comet.account`;
-- MySQL dump 10.13  Distrib 8.0.12, for Win64 (x86_64)
--
-- Host: localhost    Database: comet.account
-- ------------------------------------------------------
-- Server version	8.0.12

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
 SET NAMES utf8 ;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `account`
--

DROP TABLE IF EXISTS `account`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `account` (
  `AccountID` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Username` varchar(16) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Password` varchar(70) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Salt` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `AuthorityID` smallint(6) unsigned NOT NULL DEFAULT '1',
  `StatusID` smallint(6) unsigned NOT NULL DEFAULT '1',
  `Name` varchar(70) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `Email` varchar(70) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `IPAddress` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `Registered` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`AccountID`),
  UNIQUE KEY `AccountID_UNIQUE` (`AccountID`),
  UNIQUE KEY `Username_UNIQUE` (`Username`),
  KEY `fk_account_account_authority_idx` (`AuthorityID`),
  KEY `fk_account_account_status_idx` (`StatusID`),
  CONSTRAINT `fk_account_account_authority` FOREIGN KEY (`AuthorityID`) REFERENCES `account_authority` (`authorityid`),
  CONSTRAINT `fk_account_account_status` FOREIGN KEY (`StatusID`) REFERENCES `account_status` (`statusid`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `account`
--

LOCK TABLES `account` WRITE;
/*!40000 ALTER TABLE `account` DISABLE KEYS */;
INSERT INTO `account` VALUES (1,'Spirited','6bbb9dc119596a2397ed7b13ff8d47431ff6c8ef1a9d82d05928daecd7f1d5b8','3c1d7ae9a3080cba8dad6b0bf4d50e2f',1,1,NULL,NULL,NULL,'2018-09-26 00:17:40');
/*!40000 ALTER TABLE `account` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `account_passwordhash` BEFORE INSERT ON `account` FOR EACH ROW BEGIN
    --
	-- Name:   Password Hash
	-- Author: Gareth Jensen (Spirited)
	-- Date:   2018-09-25
	--
	-- Description:
	-- When a plain text password without a hash or salt has been inserted into the database 
	-- along with a new account, then the plain text password will be hashed and a salt will
	-- be generated from a random MD5 string. Due to client limitations, passwords cannot be
    -- longer than 16 characters.
	-- 
	IF (NEW.`Salt` IS NULL) THEN
    
		IF (LENGTH(NEW.`Password`) > 16) THEN
			SET NEW.`Password` = NULL;
        END IF;
        
        SET NEW.`Salt` = MD5(RAND());
        SET NEW.`Password` = SHA2(CONCAT(NEW.`Password`, NEW.`Salt`), 256);
    END IF;
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `account_authority`
--

DROP TABLE IF EXISTS `account_authority`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `account_authority` (
  `AuthorityID` smallint(5) unsigned NOT NULL AUTO_INCREMENT,
  `AuthorityName` varchar(45) NOT NULL,
  PRIMARY KEY (`AuthorityID`),
  UNIQUE KEY `AuthorityID_UNIQUE` (`AuthorityID`),
  UNIQUE KEY `AuthorityName_UNIQUE` (`AuthorityName`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `account_authority`
--

LOCK TABLES `account_authority` WRITE;
/*!40000 ALTER TABLE `account_authority` DISABLE KEYS */;
INSERT INTO `account_authority` VALUES (5,'Administrator'),(3,'Game Master'),(2,'Moderator'),(1,'Player'),(4,'Project Manager');
/*!40000 ALTER TABLE `account_authority` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `account_status`
--

DROP TABLE IF EXISTS `account_status`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `account_status` (
  `StatusID` smallint(5) unsigned NOT NULL AUTO_INCREMENT,
  `StatusName` varchar(45) NOT NULL,
  PRIMARY KEY (`StatusID`),
  UNIQUE KEY `StatusID_UNIQUE` (`StatusID`),
  UNIQUE KEY `StatusName_UNIQUE` (`StatusName`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `account_status`
--

LOCK TABLES `account_status` WRITE;
/*!40000 ALTER TABLE `account_status` DISABLE KEYS */;
INSERT INTO `account_status` VALUES (2,'Activated'),(5,'Banned'),(3,'Limited'),(4,'Locked'),(1,'Registered');
/*!40000 ALTER TABLE `account_status` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `logins`
--

DROP TABLE IF EXISTS `logins`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `logins` (
  `Timestamp` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `AccountID` int(10) unsigned NOT NULL,
  `IPAddress` varchar(45) NOT NULL,
  PRIMARY KEY (`AccountID`,`Timestamp`),
  KEY `fk_logins_account_idx` (`AccountID`),
  CONSTRAINT `fk_logins_account` FOREIGN KEY (`AccountID`) REFERENCES `account` (`accountid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `logins`
--

LOCK TABLES `logins` WRITE;
/*!40000 ALTER TABLE `logins` DISABLE KEYS */;
/*!40000 ALTER TABLE `logins` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `realm`
--

DROP TABLE IF EXISTS `realm`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `realm` (
  `RealmID` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(16) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `AuthorityID` smallint(6) unsigned NOT NULL DEFAULT '1' COMMENT 'Authority level required',
  `GameIPAddress` varchar(45) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL DEFAULT '127.0.0.1',
  `RpcIPAddress` varchar(45) COLLATE utf8_bin NOT NULL DEFAULT '127.0.0.1',
  `GamePort` int(10) unsigned NOT NULL DEFAULT '5816',
  `RpcPort` int(10) unsigned NOT NULL DEFAULT '5817',
  `RpcKey` varchar(128) COLLATE utf8_bin DEFAULT '84C86CA01B757EAD43796A4B5B589B7C7C3D14FBB010809D',
  `RpcIV` varchar(128) COLLATE utf8_bin DEFAULT '8D437D941EB323FE55A64E7CC7D07B0E',
  PRIMARY KEY (`RealmID`),
  UNIQUE KEY `RealmID_UNIQUE` (`RealmID`),
  UNIQUE KEY `Name_UNIQUE` (`Name`),
  KEY `fk_realm_account_authority_idx` (`AuthorityID`),
  CONSTRAINT `fk_realm_account_authority` FOREIGN KEY (`AuthorityID`) REFERENCES `account_authority` (`authorityid`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `realm`
--

LOCK TABLES `realm` WRITE;
/*!40000 ALTER TABLE `realm` DISABLE KEYS */;
INSERT INTO `realm` VALUES (1,'Comet',1,'127.0.0.1','127.0.0.1',5816,5817,'84C86CA01B757EAD43796A4B5B589B7C7C3D14FBB010809D','8D437D941EB323FE55A64E7CC7D07B0E');
/*!40000 ALTER TABLE `realm` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping events for database 'comet.account'
--

--
-- Dumping routines for database 'comet.account'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2018-09-29 22:36:25
