delimiter $$


CREATE DATABASE `autox` /*!40100 DEFAULT CHARACTER SET utf8 */$$

delimiter $$


CREATE TABLE `content` (

  `id` varchar(64) NOT NULL,

  `data` text NOT NULL

) ENGINE=InnoDB DEFAULT CHARSET=utf8$$



delimiter $$


CREATE TABLE `relationship` (

  `master` varchar(64) NOT NULL,

  `type` varchar(16) NOT NULL,

  `slave` varchar(64) NOT NULL

) ENGINE=InnoDB DEFAULT CHARSET=utf8$$

