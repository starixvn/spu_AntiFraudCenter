/*<info serverAlias="AFC-AFC_DataCenter" databaseType="2" executers="afcExtAPI,afcAPI" isFunction="0" isNested="0"></info>*/
DROP PROCEDURE IF EXISTS `AFC_DC_Account_CheckExist`;

DELIMITER $$
CREATE DEFINER=`afcOwner`@`%` PROCEDURE `AFC_DC_Account_CheckExist`(
		IN	ip_ParticipantID	INT
	,	IN	ip_AccountInfo 		JSON
)
    SQL SECURITY INVOKER
sp :BEGIN  
/* 
		Created:	20211111@Aries.Nguyen
		Task:		Check Exist
		DB:			AFC_DataCenter
		Original:
		Revisions:
			- 20211111@Aries.Nguyen: Created [Redmine ID: #164149]
            
        Param's Explanation (filtered by):
        
        Example:
			- CALL AFC_DC_Account_CheckExist(1,'[{"AccountID": "AccountID_Vaule_Test"}]');
	*/  
    DROP TEMPORARY TABLE IF EXISTS Temp_RawID;    
	CREATE TEMPORARY TABLE Temp_RawID( 	  
			RawID 		VARCHAR(128) PRIMARY KEY
		,	`Status` 	VARCHAR(30) 
	);
    
    DROP TEMPORARY TABLE IF EXISTS Temp_InvalidAccount;    
	CREATE TEMPORARY TABLE Temp_InvalidAccount( 	  
			RawID 	VARCHAR(128) PRIMARY KEY
	);
    
    DROP TEMPORARY TABLE IF EXISTS Temp_NotExistAccount;    
	CREATE TEMPORARY TABLE Temp_NotExistAccount( 	  
			RawID 	VARCHAR(128) PRIMARY KEY
	);
    
    INSERT IGNORE INTO Temp_RawID(RawID)
	SELECT	RawID
	FROM JSON_TABLE(ip_AccountInfo,
		 "$[*]" COLUMNS(
				RawID 			VARCHAR(128)		PATH "$.AccountID" 
		)
	) AS info;
    
    INSERT IGNORE INTO Temp_InvalidAccount(RawID)
    SELECT tmp.RawID
    FROM  Temp_RawID  AS tmp
    WHERE NOT EXISTS (SELECT 1 FROM AFC_DataCenter.Account AS acc WHERE tmp.RawID = acc.RawID AND acc.ParticipantID = ip_ParticipantID);
	
    INSERT IGNORE INTO Temp_NotExistAccount(RawID)
    SELECT RawID
    FROM  Temp_InvalidAccount  AS tmp
    WHERE NOT EXISTS (SELECT 1 FROM AFC_DataCenter.Account AS acc WHERE tmp.RawID = acc.RawID);
    
    UPDATE Temp_RawID AS raw 
    SET raw.`Status` = 'Not Exist'
    WHERE EXISTS (SELECT 1 FROM Temp_NotExistAccount AS tmp WHERE tmp.RawID = raw.RawID);
    
    UPDATE Temp_RawID AS raw 
    SET raw.`Status` = 'No Permission Account'
    WHERE EXISTS (SELECT 1 FROM Temp_InvalidAccount AS tmp WHERE tmp.RawID = raw.RawID)
		AND raw.`Status` IS NULL;
	
    UPDATE Temp_RawID AS raw 
    SET raw.`Status` = 'Valid'
    WHERE raw.`Status` IS NULL;
    
    SELECT 	RawID AS AccountID
		,	`Status`
    FROM Temp_RawID;
    
END$$
DELIMITER ;

