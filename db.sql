-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server version:               5.5.8-log - MySQL Community Server (GPL)
-- Server OS:                    Win32
-- HeidiSQL Version:             9.3.0.5055
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;


-- Dumping database structure for wifi
CREATE DATABASE IF NOT EXISTS `wifi` /*!40100 DEFAULT CHARACTER SET latin1 */;
USE `wifi`;

-- Dumping structure for table wifi.data
CREATE TABLE IF NOT EXISTS `data` (
  `id` int(11) NOT NULL,
  `ll_ref` int(11) NOT NULL,
  `mac` text NOT NULL,
  `rssi` float NOT NULL,
  `lat` double NOT NULL,
  `lon` double NOT NULL,
  `ts` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`ll_ref`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.
-- Dumping structure for table wifi.nn
CREATE TABLE IF NOT EXISTS `nn` (
  `ll_ref` bigint(20) NOT NULL,
  `nn_data` text NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.
-- Dumping structure for table wifi.routers
CREATE TABLE IF NOT EXISTS `routers` (
  `ll_ref` bigint(20) NOT NULL,
  `mac` varchar(20) NOT NULL,
  `model` tinytext NOT NULL,
  UNIQUE KEY `ll_ref` (`ll_ref`,`mac`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
