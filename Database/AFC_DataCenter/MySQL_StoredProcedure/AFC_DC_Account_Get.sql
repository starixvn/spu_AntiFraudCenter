/*<info serverAlias="AFC-AFC_DataCenter" databaseType="2" executers="afcExtAPI,afcAPI,afcWeb,afcService" isFunction="0" isNested="0"></info>*/
DROP PROCEDURE IF EXISTS `AFC_DC_Account_Get`;

DELIMITER $$
CREATE DEFINER=`afcOwner`@`%` PROCEDURE `AFC_DC_Account_Get`(
	IN	ip_AccountInfo 		JSON
)
    SQL SECURITY INVOKER
sp: BEGIN
/* 
		Created:	20211111@Aries.Nguyen  
		Task:		Add account
		DB:			AFC_DataCenter
		Original:
		Revisions:
			- 20211111@Aries.Nguyen: Created [Redmine ID: #164149]
            - 20220228@Aries.Nguyen: Multiple bank account [Redmine ID: #164149]
            - 20230109@Long.Luu: Support Account's IP & Device [Redmine ID: #0000]
            
        Param's Explanation (filtered by):
        
        Example:
			- CALL AFC_DC_Account_Get('{"AccountID": "AccountID_Vaule_Test", "Phone": "Phone_Vaule_Test", "CountryCode":"+4", "IP":"10.40.40.9"}');
			- CALL AFC_DC_Account_Get('{"IP":"10.40.40.22"}');
	*/   

    DECLARE lv_AccountID 		VARCHAR(128);
    DECLARE lv_FullName 		VARCHAR(128);
    DECLARE lv_Phone 			VARCHAR(128);
    DECLARE lv_CountryCode 		VARCHAR(128);
    DECLARE lv_Email 			VARCHAR(128);
    DECLARE lv_DocumentID 		VARCHAR(128);
    DECLARE lv_DocumentTypeID 	INT;
    DECLARE lv_BankName 		VARCHAR(128);
    DECLARE lv_BankAccountID 	VARCHAR(128);
    DECLARE lv_IP		 		LONGTEXT;
    DECLARE lv_Device 			LONGTEXT;
	
	DROP TEMPORARY TABLE IF EXISTS Temp_AccountIDByBankAccount;    
	CREATE TEMPORARY TABLE Temp_AccountIDByBankAccount( 	
			RawID			VARCHAR(128) NOT NULL
		,	ParticipantID	INT UNSIGNED
		,	PRIMARY KEY(RawID,ParticipantID)
	);
    
    DROP TEMPORARY TABLE IF EXISTS Temp_AccountIDByIP;    
	CREATE TEMPORARY TABLE Temp_AccountIDByIP( 	
			RawID			VARCHAR(128) NOT NULL
		,	ParticipantID	INT UNSIGNED
		,	PRIMARY KEY(RawID,ParticipantID)
	);
    
    DROP TEMPORARY TABLE IF EXISTS Temp_AccountIDByDevice;    
	CREATE TEMPORARY TABLE Temp_AccountIDByDevice( 	
			RawID			VARCHAR(128) NOT NULL
		,	ParticipantID	INT UNSIGNED
		,	PRIMARY KEY(RawID,ParticipantID)
	);
    
    DROP TEMPORARY TABLE IF EXISTS Temp_IP;    
	CREATE TEMPORARY TABLE Temp_IP( 	
			IP				VARCHAR(39) PRIMARY KEY
	);
    
    DROP TEMPORARY TABLE IF EXISTS Temp_Device;    
	CREATE TEMPORARY TABLE Temp_Device( 	
			Device			VARCHAR(128) NOT NULL PRIMARY KEY
	);
    
    SELECT JSON_VALUE(ip_AccountInfo, "$.AccountID")		INTO lv_AccountID;
    SELECT JSON_VALUE(ip_AccountInfo, "$.FullName")			INTO lv_FullName;
    SELECT JSON_VALUE(ip_AccountInfo, "$.Phone") 			INTO lv_Phone;
    SELECT JSON_VALUE(ip_AccountInfo, "$.CountryCode")		INTO lv_CountryCode;
    SELECT JSON_VALUE(ip_AccountInfo, "$.Email")			INTO lv_Email;
    SELECT JSON_VALUE(ip_AccountInfo, "$.DocumentID")		INTO lv_DocumentID;
    SELECT JSON_VALUE(ip_AccountInfo, "$.DocumentTypeID") 	INTO lv_DocumentTypeID;
    SELECT JSON_VALUE(ip_AccountInfo, "$.BankName")			INTO lv_BankName ;
    SELECT JSON_VALUE(ip_AccountInfo, "$.BankAccountID") 	INTO lv_BankAccountID;
    SELECT JSON_VALUE(ip_AccountInfo, "$.IP") 				INTO lv_IP;
    SELECT JSON_VALUE(ip_AccountInfo, "$.Device") 			INTO lv_Device;
    
    IF(lv_AccountID IS NULL AND lv_FullName IS NULL AND lv_Phone IS NULL AND lv_CountryCode IS NULL AND lv_Email IS NULL AND lv_DocumentID IS NULL AND lv_DocumentTypeID IS NULL 
		AND lv_BankName IS NULL AND lv_BankAccountID IS NULL AND lv_IP IS NULL AND lv_Device IS NULL) THEN
		LEAVE sp;
    END IF;
    
    IF (lv_BankAccountID IS NOT NULL) THEN
		INSERT INTO Temp_AccountIDByBankAccount(RawID, ParticipantID)
        SELECT  acc.RawID
			,	acc.ParticipantID
        FROM BankAccount AS acc
        WHERE 	acc.BankAccountID = lv_BankAccountID
			AND acc.BankName = lv_BankName;
    END IF;
    
    IF (lv_IP IS NOT NULL) THEN
		SET @sql = CONCAT("INSERT INTO Temp_IP (IP) VALUES ('", REPLACE(lv_IP, ",", "'),('"),"');");
		PREPARE stmt1 FROM @sql;
		EXECUTE stmt1;
    
		INSERT INTO Temp_AccountIDByIP(RawID, ParticipantID)
        SELECT  acc.RawID
			,	acc.ParticipantID
        FROM Temp_IP AS t
			INNER JOIN AccountIP AS acc	ON t.IP = acc.IP;
    END IF;
    
    IF (lv_Device IS NOT NULL) THEN
		SET @sql = CONCAT("INSERT INTO Temp_Device (Device) VALUES ('", REPLACE(lv_Device, ",", "'),('"),"');");
		PREPARE stmt1 FROM @sql;
		EXECUTE stmt1;
        
		INSERT INTO Temp_AccountIDByDevice(RawID, ParticipantID)
        SELECT  acc.RawID
			,	acc.ParticipantID
        FROM Temp_IP AS t
			INNER JOIN AccountDevice AS acc	ON t.Device = acc.Device;
    END IF;
    
    SELECT 	acc.RawID AS AccountID
		,	acc.FullName 
        ,	par.ParticipantName
        ,	acc.Phone
        ,	acc.CountryCode
        ,	acc.Email
        ,	acc.DocumentID
        ,	acc.DocumentTypeID
        ,	(SELECT JSON_ARRAYAGG(JSON_OBJECT('BankName', bank.BankName,'BankAccountID', bank.BankAccountID)) 
			 FROM BankAccount AS bank 
             WHERE  bank.RawID = acc.RawID 
				AND bank.ParticipantID = acc.ParticipantID) AS BankInfo
        ,	REGEXP_REPLACE(JSON_UNQUOTE(acc.FraudID), '[\\[\\] ]', '') AS FraudID
		,	acc.Modified
		,	acc.Created
	FROM AFC_DataCenter.Account AS acc
		LEFT JOIN AFC_DataCenter.Participant AS par ON acc.ParticipantID = par.ParticipantID
	WHERE 	(lv_AccountID	IS NULL OR acc.RawID = lv_AccountID)
		AND	(lv_FullName	IS NULL OR acc.FullName = lv_FullName)
		AND	(lv_Email 		IS NULL	OR acc.Email = lv_Email)
		AND	(lv_Phone 		IS NULL	OR (acc.Phone = lv_Phone AND  acc.CountryCode = lv_CountryCode))
        AND	(lv_DocumentID 	IS NULL	OR (acc.DocumentID = lv_DocumentID AND  acc.DocumentTypeID = lv_DocumentTypeID))
        AND	(lv_BankAccountID 	IS NULL	 OR ((acc.RawID,acc.ParticipantID) IN (SELECT tmp.RawID, tmp.ParticipantID FROM Temp_AccountIDByBankAccount AS tmp)))
        AND	(lv_IP 			IS NULL	OR ((acc.RawID,acc.ParticipantID) IN (SELECT tmp.RawID, tmp.ParticipantID FROM Temp_AccountIDByIP AS tmp)))
        AND	(lv_Device 		IS NULL	OR ((acc.RawID,acc.ParticipantID) IN (SELECT tmp.RawID, tmp.ParticipantID FROM Temp_AccountIDByDevice AS tmp)));

END$$
DELIMITER ;

