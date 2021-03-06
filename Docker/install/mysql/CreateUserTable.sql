CREATE SCHEMA IF NOT EXISTS `YONGJIN`;
USE YONGJIN;

CREATE TABLE `Users`(
    userID varchar(50) NOT NULL PRIMARY KEY,  
    salt varchar(50),
    hashingPW varchar(50)
);

CREATE TABLE `MailBox`(
    MailBoxID varchar(50) NOT NULL,
    senderID varchar(50),
    kind int,
    Item varchar(50) NOT NULL,
    ItemCount int,
    PRIMARY KEY (MailBoxID, Item)
);