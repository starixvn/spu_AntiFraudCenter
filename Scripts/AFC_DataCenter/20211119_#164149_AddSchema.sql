/*<info serverAlias="AFCMain-AFC_DataCenter" databaseType="2"></info>*/
/*
	Created: 20211119@Aries.Nguyen [Redmine ID: #164149]
	Task: Add schema
	DB: AFC_DataCenter
*/

CREATE DATABASE AFC_DataCenter;

USE AFC_DataCenter;
CREATE TABLE `Account` (
   `ID` bigint unsigned NOT NULL AUTO_INCREMENT,
   `ParticipantID` int NOT NULL,
   `RawID` varchar(128) DEFAULT NULL,
   `FullName` varchar(128) DEFAULT NULL,
   `Phone` varchar(128) DEFAULT NULL,
   `CountryCode` varchar(5) DEFAULT NULL,
   `Email` varchar(128) DEFAULT NULL,
   `DocumentID` varchar(128) DEFAULT NULL,
   `DocumentTypeID` int DEFAULT NULL,
   `BankName` varchar(128) DEFAULT NULL,
   `BankAccountID` varchar(128) DEFAULT NULL,
   `FraudID` int DEFAULT NULL,
   `Created` datetime DEFAULT CURRENT_TIMESTAMP,
   `Modified` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
   PRIMARY KEY (`ID`),
   KEY `IX_Account_RawID` (`RawID`),
   KEY `IX_Account_FullName` (`FullName`),
   KEY `IX_Account_Phone` (`Phone`),
   KEY `IX_Account_Email` (`Email`),
   KEY `IX_Account_DocumentID` (`DocumentID`),
   KEY `IX_Account_BankAccountID` (`BankAccountID`)
 ) ENGINE=InnoDB;
 
CREATE TABLE `Account_Log` (
   `ID` bigint unsigned NOT NULL AUTO_INCREMENT,
   `ParticipantID` int NOT NULL,
   `RawID` varchar(128) DEFAULT NULL,
   `FullName` varchar(128) DEFAULT NULL,
   `Phone` varchar(128) DEFAULT NULL,
   `CountryCode` varchar(5) DEFAULT NULL,
   `Email` varchar(128) DEFAULT NULL,
   `DocumentID` varchar(128) DEFAULT NULL,
   `DocumentTypeID` int DEFAULT NULL,
   `BankName` varchar(128) DEFAULT NULL,
   `BankAccountID` varchar(128) DEFAULT NULL,
   `FraudID` int DEFAULT NULL,
   `IsDeleted` bit(1) DEFAULT b'0',
   `IsUpdated` bit(1) DEFAULT b'0',
   `Created` datetime DEFAULT CURRENT_TIMESTAMP,
   PRIMARY KEY (`ID`)
 ) ENGINE=InnoDB;
 
 CREATE TABLE `DocumentType` (
   `DocumentTypeID` int NOT NULL AUTO_INCREMENT,
   `DocumentTypeName` varchar(100) NOT NULL,
   PRIMARY KEY (`DocumentTypeID`)
 ) ENGINE=InnoDB;

 CREATE TABLE `FraudCase` (
   `FraudID` int NOT NULL AUTO_INCREMENT,
   `FraudName` varchar(100) NOT NULL,
   PRIMARY KEY (`FraudID`)
 ) ENGINE=InnoDB;
 
CREATE TABLE `Participant` (
   `ParticipantID` int NOT NULL AUTO_INCREMENT,
   `ParticipantName` varchar(100) NOT NULL,
   `SecretKey` varchar(128) NOT NULL,
   `IPs` varchar(200) DEFAULT NULL,
   `IsActive` bit(1) DEFAULT 1,
   `Created` datetime DEFAULT CURRENT_TIMESTAMP,
   PRIMARY KEY (`ParticipantID`)
 ) ENGINE=InnoDB;
 
CREATE TABLE `Participant_Log` (
   `ID` bigint unsigned NOT NULL AUTO_INCREMENT,
   `ParticipantID` int NOT NULL,
   `SecretKey` varchar(128) NOT NULL,
   `IsUpdated` bit(1) DEFAULT 0,
   `Created` datetime DEFAULT CURRENT_TIMESTAMP,
   PRIMARY KEY (`ID`)
 ) ENGINE=InnoDB;


 