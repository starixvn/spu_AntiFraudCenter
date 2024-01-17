/*<info serverAlias="AFC-AFC_DataCenter" databaseType="2" executers="afcExtAPIAdmin,afcAPIAdmin,afcWebAdmin,afcServiceAdmin" isFunction="0" isNested="0"></info>*/
DROP PROCEDURE IF EXISTS `AFC_DC_Account_Insert`;

DELIMITER $$
CREATE DEFINER=`afcOwner`@`%` PROCEDURE `AFC_DC_Account_Insert`(
		IN	ip_ParticipantID	INT
	,	IN	ip_AccountInfo 		JSON
    
    ,	OUT	op_ErrorMessage		VARCHAR(100)
)
    SQL SECURITY INVOKER
sp :BEGIN  
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
			- CALL AFC_DC_Account_Insert(1,'[{"AccountID": "1234",
											  "FullName": "FullName_Value",
                                              "Phone":  "Phone_Value",
                                              "CountryCode":"CountryCode_Value",
                                              "Email": "Email_Value",
                                              "DocumentID": "DocumentID_Value",
                                              "DocumentTypeID": 1,
                                              "BankInfo": [{"BankName": "BankName_Vaule_Test","BankAccountID": "BankAccountID_Vaule_Test"}],
                                              "IP": "10.40.40.22,10.90.90.9",
                                              "Device": "AOHKFJKSGFYI,AIOU1689IUAGKL,UIQGC0AH1KJH78A",
                                              "FraudID": [1,2,3]}]', @mess); select @mess;
	*/  
    DECLARE lv_CheckValid INT;
    
    DROP TEMPORARY TABLE IF EXISTS Temp_AccountExisted;    
	CREATE TEMPORARY TABLE Temp_AccountExisted( 	  
			RawID 	VARCHAR(128) PRIMARY KEY
	);
    
    DROP TEMPORARY TABLE IF EXISTS Temp_FraudID;    
	CREATE TEMPORARY TABLE Temp_FraudID( 	  
			FraudID 	INT PRIMARY KEY
	);
    
	DROP TEMPORARY TABLE IF EXISTS Temp_Account;    
	CREATE TEMPORARY TABLE Temp_Account( 	   
			ID   			BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY
		,	RawID 			VARCHAR(128)
		,	ParticipantID	INT
		,	FullName 		NVARCHAR(128)
		,	Phone 			VARCHAR(128)
		,	CountryCode 	VARCHAR(8)
		,	Email 			VARCHAR(128)
		,	DocumentID  	VARCHAR(128)
		,	DocumentTypeID  INT
        ,	BankInfo		JSON
        ,	IP				LONGTEXT
        ,	Device			LONGTEXT
		,	FraudID			JSON
	);
    
    DROP TEMPORARY TABLE IF EXISTS Temp_BankInfo;    
	CREATE TEMPORARY TABLE Temp_BankInfo( 	
			ID				INT NOT NULL AUTO_INCREMENT PRIMARY KEY
		,	RawID 			VARCHAR(128)
		,	ParticipantID	INT
		,	BankName		VARCHAR(128)
		,	BankAccountID	VARCHAR(128)
	);    
    
	INSERT IGNORE INTO Temp_Account(RawID, ParticipantID, FullName, Phone, CountryCode, Email, DocumentID, DocumentTypeID, BankInfo, IP, Device, FraudID)
	SELECT	RawID
		, 	ip_ParticipantID
		, 	FullName
		, 	Phone
        , 	CountryCode
        , 	Email
        , 	DocumentID
        , 	DocumentTypeID
        , 	BankInfo
        ,	IP
        ,	Device
        , 	FraudID
	FROM JSON_TABLE(ip_AccountInfo,
		 "$[*]" COLUMNS(
				RawID 			VARCHAR(128)	PATH "$.AccountID" 
			,	FullName 		VARCHAR(128)	PATH "$.FullName" 
			,	Phone 			VARCHAR(128)	PATH "$.Phone" 
			,	CountryCode 	VARCHAR(8)		PATH "$.CountryCode" 
			,	Email 			VARCHAR(128)	PATH "$.Email" 
			,	DocumentID  	VARCHAR(128)	PATH "$.DocumentID" 
			,	DocumentTypeID  TINYINT			PATH "$.DocumentTypeID" 
            ,	BankInfo		JSON			PATH "$.BankInfo" 
            ,	IP				LONGTEXT		PATH "$.IP" 
            ,	Device			LONGTEXT		PATH "$.Device" 
			,	FraudID			JSON			PATH "$.FraudID" 
		)
	) AS info;
    
    INSERT IGNORE INTO Temp_FraudID(FraudID)
    SELECT temp.FraudID
	FROM JSON_TABLE(ip_AccountInfo,
          '$[*]' COLUMNS(
              NESTED PATH '$.FraudID[*]' COLUMNS (FraudID INT PATH '$')
          )
	) AS temp;
    
    INSERT IGNORE INTO Temp_AccountExisted(RawID)
    SELECT tmp.RawID 
    FROM  Temp_Account AS tmp 
    WHERE EXISTS (SELECT 1 
				  FROM AFC_DataCenter.Account AS acc 
				  WHERE acc.RawID = tmp.RawID 
					AND acc.ParticipantID = ip_ParticipantID);
	
    IF EXISTS (SELECT 1 FROM Temp_Account WHERE RawID IS NULL OR RawID = '')	THEN
		SET op_ErrorMessage = "RawID is not null or empty";
        
        LEAVE sp;
    END IF;
    
    IF EXISTS (SELECT 1 FROM Temp_AccountExisted)	THEN
		SELECT GROUP_CONCAT(RawID)
        INTO op_ErrorMessage
        FROM Temp_AccountExisted;
		SET op_ErrorMessage = CONCAT("Existed Data: ", op_ErrorMessage);
        
        LEAVE sp;
    END IF;
    
    SELECT 1 
    INTO lv_CheckValid
    FROM Temp_Account AS tmp 
	WHERE NOT EXISTS (SELECT 1 FROM DocumentType AS doc WHERE doc.DocumentTypeID = tmp.DocumentTypeID) 
		AND tmp.DocumentTypeID IS NOT NULL
	LIMIT 1;
        
	IF lv_CheckValid = 1 THEN
		SET op_ErrorMessage = "DocumentTypeID invalid";
        
        LEAVE sp;
    END IF;
    
    SELECT 1 
    INTO lv_CheckValid
    FROM Temp_FraudID AS tmp 
	WHERE NOT EXISTS (SELECT 1 FROM FraudCase AS fr WHERE fr.FraudID = tmp.FraudID)
    LIMIT 1;
        
	IF lv_CheckValid = 1 THEN
		SET op_ErrorMessage = "FraudID invalid";
        
        LEAVE sp;
    END IF;

    INSERT INTO Temp_BankInfo(RawID, ParticipantID, BankName, BankAccountID) 
    SELECT 	tmp.RawID
		,	tmp.ParticipantID
        ,	tbl.BankName
        ,	tbl.BankAccountID
    FROM Temp_Account AS tmp
    JOIN JSON_TABLE(tmp.BankInfo, 
		'$[*]' COLUMNS( 
			NESTED PATH '$' COLUMNS (
											BankName 		VARCHAR(128) PATH '$.BankName'
										,	BankAccountID 	VARCHAR(128) PATH '$.BankAccountID'
									)
		)
	) AS tbl;    
    
    INSERT INTO AFC_DataCenter.Account(RawID, ParticipantID, FullName, Phone, CountryCode, Email, DocumentID, DocumentTypeID, FraudID)
    SELECT 	RawID
		, 	ParticipantID
		, 	FullName
		, 	Phone
        , 	CountryCode
        , 	Email
        , 	DocumentID
        , 	DocumentTypeID
        , 	FraudID
    FROM Temp_Account;
    
    INSERT IGNORE INTO AFC_DataCenter.BankAccount(RawID,ParticipantID,BankName,BankAccountID)
    SELECT 	temp.RawID
		,	temp.ParticipantID
		,	temp.BankName
        ,	temp.BankAccountID
    FROM Temp_BankInfo AS temp
    WHERE 	temp.BankName IS NOT NULL 
		AND temp.BankAccountID IS NOT NULL;
        
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
        
	INSERT IGNORE INTO AFC_DataCenter.AccountDevice(RawID,ParticipantID,Device)
    SELECT a.RawID, ip_ParticipantID,
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
    
END$$
DELIMITER ;

