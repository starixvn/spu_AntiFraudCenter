/*<info serverAlias="AFC-AFC_DataCenter" databaseType="2" executers="afcExtAPIAdmin,afcAPIAdmin,afcWebAdmin,afcServiceAdmin" isFunction="0" isNested="0"></info>*/
USE AFC_DataCenter;

DROP PROCEDURE IF EXISTS `AFC_DC_Account_Scan`;

DELIMITER $$
CREATE DEFINER=`afcOwner`@`%` PROCEDURE `AFC_DC_Account_Scan`(
		IN	ip_ParticipantID	INT
	,	IN	ip_ParticipantList	LONGTEXT
    ,	IN  ip_FromDate			DATETIME
    ,	IN  ip_ToDate			DATETIME 
    
    ,	OUT op_IsContinue		SMALLINT
    ,	OUT op_FromDate			DATETIME
    ,	OUT op_ToDate			DATETIME
)
    SQL SECURITY INVOKER
sp: BEGIN
/* 
		Created:	20211227@Aries.Nguyen  
		Task:		Scanning Account
		DB:			AFC_DataCenter
		Original:
		Revisions:
			- 20211227@Aries.Nguyen: Created [Redmine ID: #164149]
			- 20220228@Aries.Nguyen: Multiple bank account [Redmine ID: #164149]
            - 20230109@Long.Luu: Support Account's IP & Device [Redmine ID: #0000]
            
        Param's Explanation (filtered by):
        
        Example:
			- CALL AFC_DataCenter.AFC_DC_Account_Scan(9,'par7','2021-12-01 00:00:00', '2021-12-02 22:02:20',@op_IsContinue, @op_FromDate, @op_ToDate);
	*/ 
    DECLARE CONS_SysID_BatchSize 	INT DEFAULT 1;
    
    DECLARE lv_IsFilterAllPar 		BIT DEFAULT 1;
    DECLARE lv_BatchSize 			INT;
    DECLARE lv_LastScanDate 		DATETIME;
    DECLARE lv_MinScanDate 			DATETIME;
    DECLARE lv_Flag 				INT;
    
    DROP TEMPORARY TABLE IF EXISTS Temp_Account;
	CREATE TEMPORARY TABLE Temp_Account (
			ID 					BIGINT UNSIGNED PRIMARY KEY
		,	AccountID			VARCHAR(128)
		,	ParticipantName 	VARCHAR(100)
		,	FullName 			VARCHAR(128)
		,	Phone 				VARCHAR(128)
		,	CountryCode 		VARCHAR(5)
		,	Email				VARCHAR(128)
		,	DocumentID 			VARCHAR(128) 
		,	DocumentTypeID 		INT 
		,	BankInfo 			JSON
		,	FraudID 			VARCHAR(200)  
		,	Created 			DATETIME 
		,	Modified 			DATETIME 
		,	INDEX				IX_Temp_Account_Modified(Modified) 
	 );
    
    DROP TEMPORARY TABLE IF EXISTS Temp_ParticipantName;
    CREATE TEMPORARY TABLE Temp_ParticipantName (
			ParticipantName  VARCHAR(100) PRIMARY KEY
    );
    
    DROP TEMPORARY TABLE IF EXISTS Temp_ParticipantID;
    CREATE TEMPORARY TABLE Temp_ParticipantID (
			ParticipantID  INT PRIMARY KEY
    );
    
    SELECT ParameterValue
    INTO lv_BatchSize
    FROM AFC_DataCenter.SystemParameter
    WHERE ParameterID = CONS_SysID_BatchSize;
    
    SELECT LastScanDate
    INTO lv_LastScanDate
    FROM AFC_DataCenter.Participant
    WHERE ParticipantID = ip_ParticipantID;
	
	IF lv_LastScanDate IS NULL THEN
		SELECT MIN(Modified)
		INTO lv_LastScanDate
		FROM AFC_DataCenter.Account;
		
		SET lv_LastScanDate =  DATE_SUB(lv_LastScanDate, INTERVAL 1 DAY);  
	END IF;
    
    IF ip_ParticipantList IS NOT NULL AND ip_ParticipantList != '' THEN
		SET @sql = CONCAT("INSERT IGNORE INTO Temp_ParticipantName (ParticipantName) VALUES ('", REPLACE(ip_ParticipantList, ",", "'),('"),"');");
		PREPARE stmt1 FROM @sql;
		EXECUTE stmt1;
		
		INSERT IGNORE INTO Temp_ParticipantID(ParticipantID)
		SELECT pa.ParticipantID
		FROM AFC_DataCenter.Participant AS pa
		WHERE pa.ParticipantName IN (SELECT na.ParticipantName FROM Temp_ParticipantName AS na);
        
        SET lv_IsFilterAllPar = 0;
	END IF;
    
    SET op_IsContinue = 0;
	
    INSERT INTO Temp_Account(ID, AccountID, ParticipantName, FullName, Phone, CountryCode, Email, DocumentID, DocumentTypeID, BankInfo, FraudID, Modified, Created)
    SELECT 	acc.ID
		,	acc.RawID AS AccountID
		,	par.ParticipantName
		,	acc.FullName
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
		INNER JOIN AFC_DataCenter.Participant AS par ON acc.ParticipantID = par.ParticipantID
	WHERE   ((ip_FromDate IS NULL AND acc.Modified > lv_LastScanDate) 
			OR (ip_FromDate IS NOT NULL AND acc.Modified > ip_FromDate))
		AND (ip_ToDate IS NULL OR (ip_ToDate IS NOT NULL AND acc.Modified <= ip_ToDate))
        AND (lv_IsFilterAllPar = 1 OR (lv_IsFilterAllPar = 0 AND acc.ParticipantID IN (SELECT pa.ParticipantID FROM Temp_ParticipantID AS pa)))
	ORDER BY acc.Modified ASC
	LIMIT lv_BatchSize; 

    IF NOT EXISTS (SELECT 1 FROM Temp_Account) THEN
		SET op_IsContinue = 0;
        #SELECT 'No Data';
        LEAVE sp; 
    END IF;
    
	SELECT MIN(Modified)
	INTO op_FromDate
	FROM Temp_Account;
        
	SELECT MAX(Modified)
	INTO op_ToDate
	FROM Temp_Account;
    
    SELECT 	1
    INTO lv_Flag
	FROM AFC_DataCenter.Account AS acc
	WHERE   acc.Modified > op_ToDate
		AND (ip_ToDate IS NULL OR (ip_ToDate IS NOT NULL AND acc.Modified <= ip_ToDate))
		AND (lv_IsFilterAllPar = 1 OR (lv_IsFilterAllPar = 0 AND acc.ParticipantID IN (SELECT pa.ParticipantID FROM Temp_ParticipantID AS pa)))
	LIMIT 1;
    
    IF lv_Flag IS NOT NULL THEN
        SET op_IsContinue = 1;
    END IF;
    
    INSERT IGNORE INTO Temp_Account(ID, AccountID, ParticipantName, FullName, Phone, CountryCode, Email, DocumentID, DocumentTypeID, BankInfo, FraudID, Modified, Created)
    SELECT 	acc.ID
		,	acc.RawID AS AccountID
		,	par.ParticipantName
		,	acc.FullName
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
		INNER JOIN AFC_DataCenter.Participant AS par ON acc.ParticipantID = par.ParticipantID
	WHERE  acc.Modified = op_ToDate
        AND (lv_IsFilterAllPar = 1 OR acc.ParticipantID IN (SELECT pa.ParticipantID FROM Temp_ParticipantID AS pa));
	
    SELECT 	t.ID
		, 	t.AccountID
        , 	t.ParticipantName
        , 	t.FullName
        , 	t.Phone
        , 	t.CountryCode
        , 	t.Email
        , 	t.DocumentID
        , 	t.DocumentTypeID
        , 	t.BankInfo
        ,	t.IP
        ,	t.Device
        , 	t.FraudID
        , 	t.Modified
        , 	t.Created
		,	GROUP_CONCAT(ip.IP) AS IP
		,	GROUP_CONCAT(d.Device) AS Device
    FROM Temp_Account AS t		
		INNER JOIN AFC_DataCenter.AccountIP AS ip ON ip.RawID = t.RawID AND ip.ParticipantID = t.ParticipantID
		INNER JOIN AFC_DataCenter.AccountDevice AS d ON d.RawID = t.RawID AND d.ParticipantID = t.ParticipantID
	GROUP BY t.ID
		, 	t.AccountID
        , 	t.ParticipantName
        , 	t.FullName
        , 	t.Phone
        , 	t.CountryCode
        , 	t.Email
        , 	t.DocumentID
        , 	t.DocumentTypeID
        , 	t.BankInfo
        ,	t.IP
        ,	t.Device
        , 	t.FraudID
        , 	t.Modified
        , 	t.Created
    ORDER BY t.Modified ASC;
    
    IF ip_FromDate IS NULL THEN
		UPDATE AFC_DataCenter.Participant
        SET LastScanDate = op_ToDate
        WHERE ParticipantID = ip_ParticipantID;
    END IF;
END$$
DELIMITER ;

