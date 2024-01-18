/*<info serverAlias="AFC-AFC_DataCenter" databaseType="2" executers="afcExtAPIAdmin,afcAPIAdmin,afcWebAdmin,afcServiceAdmin" isFunction="0" isNested="0"></info>*/
DROP PROCEDURE IF EXISTS `AFC_DC_Account_DeleteDevice`;

DELIMITER $$
CREATE DEFINER=`afcOwner`@`%` PROCEDURE `AFC_DC_Account_DeleteDevice`(
		IN	ip_ParticipantID	INT
	,	IN	ip_AccountInfo 		JSON
    
    ,	OUT	op_ErrorMessage		VARCHAR(100)
)
    SQL SECURITY INVOKER
sp :BEGIN  
/* 
		Created:	20230112@Long.Luu
		Task:		Delete account's IPs
		DB:			AFC_DataCenter
		Original:
		Revisions:
			- 20230112@Long.Luu: Created [Redmine ID: #0000]
            
        Param's Explanation (filtered by):
        
        Example:
			- CALL AFC_DC_Account_DeleteDevice(1,'[{"AccountID": "1234",
                                              "Device": "AOHKFJKSGFYI"},
                                              {"AccountID": "5678",
                                              "Device": "UIQGC0AH1KJH78A,XZYZAH1KABC8A"}]', @mess); select @mess;
	*/  
    DROP TEMPORARY TABLE IF EXISTS Temp_AccountNotExisted;    
	CREATE TEMPORARY TABLE Temp_AccountNotExisted( 	  
			RawID 	VARCHAR(128) PRIMARY KEY
	);
    
	DROP TEMPORARY TABLE IF EXISTS Temp_Account;    
	CREATE TEMPORARY TABLE Temp_Account( 	   
			ID   			BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY
		,	RawID 			VARCHAR(128)
        ,	Device			LONGTEXT
	);
    
    DROP TEMPORARY TABLE IF EXISTS Temp_AccountDevice;    
	CREATE TEMPORARY TABLE Temp_AccountDevice( 	   
			RawID 			VARCHAR(128)
        ,	Device			VARCHAR(128)
	);
    
	INSERT IGNORE INTO Temp_Account(RawID, Device)
	SELECT	RawID
		, 	Device
	FROM JSON_TABLE(ip_AccountInfo,
		 "$[*]" COLUMNS(
				RawID 			VARCHAR(128)	PATH "$.AccountID" 
            ,	Device			LONGTEXT		PATH "$.Device" 
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
    
    IF EXISTS (SELECT 1 FROM Temp_AccountNotExisted)	THEN
		SELECT GROUP_CONCAT(RawID)
        INTO op_ErrorMessage
        FROM Temp_AccountNotExisted;
        
		SET op_ErrorMessage = CONCAT("Not Existed Account(s): ", op_ErrorMessage);
        
        LEAVE sp;
    END IF;
    
    INSERT IGNORE INTO Temp_AccountDevice(RawID,Device)
    SELECT a.RawID, 
		REPLACE(substring(substring_index(Device, ',', i),
			   length(substring_index(Device, ',', i - 1)) + 1), ',', '') AS Device
	FROM (
		SELECT a.*, @i := if((@id = a.ID), @i + 1, 1) AS i, @id := a.ID
		FROM (
				SELECT a.ID, a.RawID, a.Device
				FROM Temp_Account AS a
					INNER JOIN mysql.user AS b ON 1 = 1
				WHERE a.Device IS NOT NULL 
					AND a.Device <> ''
				ORDER BY a.ID
			) AS a
		INNER JOIN (SELECT @i := 0, @id := NULL) AS x
	) AS a
	WHERE i <= LENGTH(Device) - LENGTH(REPLACE(Device, ',', '')) + 1;
    
    DELETE d
    FROM Temp_AccountDevice AS t
		INNER JOIN AFC_DataCenter.AccountDevice AS d ON t.RawID = d.RawID 
			AND d.ParticipantID = ip_ParticipantID AND t.Device = d.Device;
    
END$$
DELIMITER ;

