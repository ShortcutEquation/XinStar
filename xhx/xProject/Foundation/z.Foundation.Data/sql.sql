CREATE TABLE `config_info` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `DKey` varchar(100) NOT NULL COMMENT '����',
  `DValue` varchar(1000) NOT NULL COMMENT 'ֵ',
  `Remark` varchar(1000) DEFAULT NULL COMMENT '����',
  `Deleted` bit(1) NOT NULL COMMENT '�Ƿ�ɾ��',
  `CreatedBy` varchar(45) NOT NULL,
  `CreatedOn` datetime NOT NULL,
  `UpdatedBy` varchar(45) DEFAULT NULL,
  `UpdatedOn` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8;