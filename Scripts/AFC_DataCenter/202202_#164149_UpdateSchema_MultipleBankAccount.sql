/*<info serverAlias="AFC-AFC_DataCenter" databaseType="2"></info>*/
/*
	Created: 20220110@Aries.Nguyen [Redmine ID: #164079]
	Task: Multiple bank account
	DB: AFC_DataCenter
*/

CREATE TABLE `BankAccount` (
	ID 				BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
	RawID			VARCHAR(128) NOT NULL,
    ParticipantID	INT UNSIGNED,
	BankName 		VARCHAR(128),
	BankAccountID 	VARCHAR(128),
	Created 		DATETIME DEFAULT CURRENT_TIMESTAMP,
    UNIQUE 			UQ_BankAccount_AccountID(RawID,ParticipantID,BankName,BankAccountID),
    INDEX 			IX_BankAccount_BankAccountID(BankAccountID)
) ENGINE=InnoDB;

ALTER TABLE Account_Log 
ADD COLUMN BankInfo JSON AFTER DocumentTypeID,
DROP COLUMN	BankName,
DROP COLUMN	BankAccountID;

ALTER TABLE Account
DROP COLUMN	BankName,
DROP COLUMN	BankAccountID;
