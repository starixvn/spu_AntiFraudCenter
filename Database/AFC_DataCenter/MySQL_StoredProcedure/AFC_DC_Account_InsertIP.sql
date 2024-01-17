/*<info serverAlias="AFC-AFC_DataCenter" databaseType="2" executers="afcExtAPIAdmin,afcAPIAdmin,afcWebAdmin,afcServiceAdmin" isFunction="0" isNested="0"></info>*/
DROP PROCEDURE IF EXISTS `AFC_DC_Account_InsertIP`;

DELIMITER $$
CREATE DEFINER=`afcOwner`@`%` PROCEDURE `AFC_DC_Account_InsertIP`(
		IN	ip_ParticipantID	INT
	,	IN	ip_AccountInfo 		JSON
    
    ,	OUT	op_ErrorMessage		VARCHAR(100)
)
    SQL SECURITY INVOKER
sp :BEGIN  
/* 
		Created:	20230112@Long.Luu
		Task:		Add account's IPs
		DB:			AFC_DataCenter
		Original:
		Revisions:
			- 20230112@Long.Luu: Created [Redmine ID: #0000]
            
        Param's Explanation (filtered by):
        
        Example:
			- CALL AFC_DC_Account_InsertIP(1,'[{"AccountID": "1234",
                                              "IP": "10.40.40.22,10.90.90.9"},
                                              {"AccountID": "5678",
                                              "IP": "10.40.40.14,10.90.90.5"}]', @mess); select @mess;
	*/      
    DROP TEMPORARY TABLE IF EXISTS Temp_AccountNotExisted;    
	CREATE TEMPORARY TABLE Temp_AccountNotExisted( 	  
			RawID 	VARCHAR(128) PRIMARY KEY
	);
    
	DROP TEMPORARY TABLE IF EXISTS Temp_Account;    
	CREATE TEMPORARY TABLE Temp_Account( 	   
			ID   			BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY
		,	RawID 			VARCHAR(128)
        ,	IP				LONGTEXT
	);
    
	INSERT IGNORE INTO Temp_Account(RawID, IP)
	SELECT	RawID
		, 	IP
	FROM JSON_TABLE(ip_AccountInfo,
		 "$[*]" COLUMNS(
				RawID 			VARCHAR(128)	PATH "$.AccountID" 
            ,	IP				LONGTEXT		PATH "$.IP" 
		)
	) AS info;
    
    INSERT IGNORE INTO Temp_AccountNotExisted(RawID)
    SELECT tmp.RawID 
    FROM  Temp_Account AS tmp 
		LEFT JOIN AFC_DataCenter.Account AS acc ON acc.RawID = tmp.RawID 
    WHERE acc.RawID IS NULL AND tmp.RawID IS NOT NULL AND tmp.RawID <> '';
	
    IF EXISTS (SELECT 1 FROM Temp_Account WHERE RawID IS NULL OR RawID = '')	THEN
		SET op_ErrorMessage = "AccountID is not null or empty";
        
        LEAVE sp;
    END IF;
    
    IF EXISTS (SELECT 1 FROM Temp_AccountExisted)	THEN
		SELECT GROUP_CONCAT(RawID)
        INTO op_ErrorMessage
        FROM Temp_AccountNotExisted;
        
		SET op_ErrorMessage = CONCAT("Not Existed Account(s): ", op_ErrorMessage);
        
        LEAVE sp;
    END IF;
        
	INSERT IGNORE INTO AFC_DataCenter.AccountIP(RawID,ParticipantID,IP)
    SELECT a.RawID, ip_ParticipantID,
		REPLACE(substring(substring_index(IP, ',', i),
			   length(substring_index(IP, ',', i - 1)) + 1), ',', '') AS IP
	FROM (
		SELECT a.*, @i := if((@id = a.ID), @i + 1, 1) AS i, @id := a.ID
		FROM (
				SELECT a.ID, a.RawID, a.IP
				FROM Temp_Account AS a
					INNER JOIN mysql.user AS b ON 1 = 1
				WHERE a.IP IS NOT NULL 
					AND a.IP <> ''
				ORDER BY a.ID
			) AS a
		INNER JOIN (SELECT @i := 0, @id := NULL) AS x
	) AS a
	WHERE i <= LENGTH(IP) - LENGTH(REPLACE(IP, ',', '')) + 1;
    
END$$
DELIMITER ;

