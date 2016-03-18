
/*

SQLyog Community v8.4 
MySQL - 5.1.46-community : Database - myrunuo

*********************************************************************

*/



/*!40101 SET NAMES utf8 */;



/*!40101 SET SQL_MODE=''*/;



/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;

/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;

/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

CREATE DATABASE /*!32312 IF NOT EXISTS*/`uoancorpwebsite_myuotest` /*!40100 DEFAULT CHARACTER SET latin1 */;



USE `uoancorpwebsite_myuotest`;



/*Table structure for table `myrunuo_characters` */



DROP TABLE IF EXISTS `myrunuo_characters`;



CREATE TABLE `myrunuo_characters` (
  `char_id` int(10) unsigned NOT NULL,
  `char_name` varchar(255) NOT NULL,
  `char_str` smallint(5) unsigned NOT NULL,
  `char_dex` smallint(6) NOT NULL,
  `char_int` smallint(6) NOT NULL,
  `char_female` tinyint(4) NOT NULL,
  `char_counts` smallint(6) NOT NULL,
  `char_guild` varchar(255) DEFAULT NULL,
  `char_guildtitle` varchar(255) DEFAULT NULL,
  `char_nototitle` varchar(255) DEFAULT NULL,
  `char_bodyhue` smallint(5) unsigned DEFAULT NULL,
  `char_public` tinyint(4) NOT NULL,
  PRIMARY KEY (`char_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;



/*Table structure for table `myrunuo_characters_layers` */



DROP TABLE IF EXISTS `myrunuo_characters_layers`;



CREATE TABLE `myrunuo_characters_layers` (
  `char_id` int(10) unsigned NOT NULL,
  `layer_id` tinyint(3) unsigned NOT NULL,
  `item_id` smallint(5) unsigned NOT NULL,
  `item_hue` smallint(5) unsigned NOT NULL,
  PRIMARY KEY (`char_id`,`layer_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;



/*Table structure for table `myrunuo_characters_skills` */



DROP TABLE IF EXISTS `myrunuo_characters_skills`;



CREATE TABLE `myrunuo_characters_skills` (
  `char_id` int(10) unsigned NOT NULL,
  `skill_id` tinyint(4) NOT NULL,
  `skill_value` smallint(5) unsigned NOT NULL,
  PRIMARY KEY (`char_id`,`skill_id`),
  KEY `skill_id` (`skill_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;



/*Table structure for table `myrunuo_guilds` */



DROP TABLE IF EXISTS `myrunuo_guilds`;



CREATE TABLE `myrunuo_guilds` (
  `guild_id` smallint(5) unsigned NOT NULL,
  `guild_name` varchar(255) NOT NULL,
  `guild_abbreviation` varchar(31) DEFAULT NULL,
  `guild_website` varchar(255) DEFAULT NULL,
  `guild_charter` varchar(255) DEFAULT NULL,
  `guild_type` varchar(8) NOT NULL,
  `guild_wars` smallint(5) unsigned NOT NULL,
  `guild_members` smallint(5) unsigned NOT NULL,
  `guild_master` int(10) unsigned NOT NULL,
  PRIMARY KEY (`guild_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;



/*Table structure for table `myrunuo_guilds_wars` */



DROP TABLE IF EXISTS `myrunuo_guilds_wars`;



CREATE TABLE `myrunuo_guilds_wars` (
  `guild_1` smallint(5) unsigned NOT NULL DEFAULT '0',
  `guild_2` smallint(5) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`guild_1`,`guild_2`),
  KEY `guild1` (`guild_1`),
  KEY `guild2` (`guild_2`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;



/*Table structure for table `myrunuo_status` */



DROP TABLE IF EXISTS `myrunuo_status`;



CREATE TABLE `myrunuo_status` (
  `character` varchar(255) NOT NULL DEFAULT '',
  PRIMARY KEY (`character`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;



/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;

/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;

/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;

/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;