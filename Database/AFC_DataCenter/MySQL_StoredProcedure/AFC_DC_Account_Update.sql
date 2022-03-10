/*<info serverAlias="AFC-AFC_DataCenter" databaseType="2" executers="afcExtAPIAdmin,afcAPIAdmin,afcWebAdmin,afcServiceAdmin" isFunction="0" isNested="0"></info>*/
DROP PROCEDURE IF EXISTS `AFC_DC_Account_Update`;

DELIMITER $$
CREATE DEFINER=`afcOwner`@`%` PROCEDURE `AFC_DC_Account_Update`(
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
			- 20211201@Aries.Nguyen: Update data from B2C to AFC [Redmine ID: #164079]
            - 20220228@Aries.Nguyen: Multiple bank account [Redmine ID: #164149]
            
        Param's Explanation (filtered by):
        
        Example:
			- CALL AFC_DC_Account_Update(1,'[{"AccountID": "AccountID_Vaule_Test","FullName": "Test_Value"}]', @mess);
	*/  
    DECLARE lv_CheckValid INT;
    
    DROP TEMPORARY TABLE IF EXISTS Temp_AccountNotExisted;    
	CREATE TEMPORARY TABLE Temp_AccountNotExisted( 	  
			RawID 	VARCHAR(128) PRIMARY KEY
	);
    
    DROP TEMPORARY TABLE IF EXISTS Temp_AccountNoChange;    
	CREATE TEMPORARY TABLE Temp_AccountNoChange( 	  
			RawID 	VARCHAR(128) PRIMARY KEY
	);
    
    DROP TEMPORARY TABLE IF EXISTS Temp_FraudID;    
	CREATE TEMPORARY TABLE Temp_FraudID( 	  
			FraudID 	INT PRIMARY KEY
	);
    
	DROP TEMPORARY TABLE IF EXISTS Temp_Account;    
	CREATE TEMPORARY TABLE Temp_Account( 	 
			AccountID 		BIGINT UNSIGNED
		,	RawID 			VARCHAR(128)
		,	ParticipantID	INT
		,	FullName 		VARCHAR(128)
		,	Phone 			VARCHAR(128)
		,	CountryCode 	VARCHAR(128)
		,	Email 			VARCHAR(128)
		,	DocumentID  	VARCHAR(128)
		,	DocumentTypeID  INT
		,	BankInfo		JSON
		,	FraudID			JSON
	);
    
    DROP TEMPORARY TABLE IF EXISTS Temp_BankInfo;    
	CREATE TEMPORARY TABLE Temp_BankInfo( 	
			ID				INT NOT NULL AUTO_INCREMENT PRIMARY KEY
		,	RawID 			VARCHAR(128)
		,	ParticipantID	INT
		,	BankName		VARCHAR(128)
		,	BankAccountID	VARCHAR(128)
        ,	INDEX			IX_Temp_BankInfo_RawID(RawID,ParticipantID)
	);
    
    DROP TEMPORARY TABLE IF EXISTS Temp_CurrBankInfo;    
	CREATE TEMPORARY TABLE Temp_CurrBankInfo( 	
			ID				INT NOT NULL AUTO_INCREMENT PRIMARY KEY
		,	RawID 			VARCHAR(128)
		,	ParticipantID	INT
		,	BankName		VARCHAR(128)
		,	BankAccountID	VARCHAR(128)
        ,	INDEX			IX_Temp_BankInfo_RawID(RawID,ParticipantID)
	);
    
    DROP TEMPORARY TABLE IF EXISTS Temp_BankInfoHasChange;    
	CREATE TEMPORARY TABLE Temp_BankInfoHasChange( 	
			RawID 			VARCHAR(128)
		,	ParticipantID	INT
        ,	PRIMARY KEY	(RawID,ParticipantID)
	);

    
	INSERT IGNORE INTO Temp_Account(RawID, ParticipantID, FullName, Phone, CountryCode, Email, DocumentID, DocumentTypeID, BankInfo, FraudID)
	SELECT	RawID
		, 	ip_ParticipantID
		, 	FullName
		, 	Phone
        , 	CountryCode
        , 	Email
        , 	DocumentID
        , 	DocumentTypeID
        , 	BankInfo
        , 	FraudID
	FROM JSON_TABLE(ip_AccountInfo,
		 "$[*]" COLUMNS(
				RawID 			VARCHAR(128)	PATH "$.AccountID"  	NULL ON EMPTY
			,	FullName 		VARCHAR(128)	PATH "$.FullName" 		NULL ON EMPTY
			,	Phone 			VARCHAR(128)	PATH "$.Phone" 			NULL ON EMPTY
			,	CountryCode 	VARCHAR(128)	PATH "$.CountryCode" 	NULL ON EMPTY
			,	Email 			VARCHAR(128)	PATH "$.Email" 			NULL ON EMPTY
			,	DocumentID  	VARCHAR(128)	PATH "$.DocumentID" 	NULL ON EMPTY
			,	DocumentTypeID  INT				PATH "$.DocumentTypeID" NULL ON EMPTY
			,	BankInfo		JSON			PATH "$.BankInfo"  		NULL ON EMPTY
			,	FraudID			JSON			PATH "$.FraudID" 		NULL ON EMPTY
		)
	) AS info;
    
    INSERT IGNORE INTO Temp_FraudID(FraudID)
    SELECT temp.FraudID
	FROM JSON_TABLE(ip_AccountInfo,
          '$[*]' COLUMNS(
              NESTED PATH '$.FraudID[*]' COLUMNS (FraudID INT PATH '$')
          )
	) AS temp
    WHERE temp.FraudID IS NOT NULL;
    
    INSERT IGNORE INTO Temp_AccountNotExisted(RawID)
    SELECT tmp.RawID 
    FROM  Temp_Account AS tmp 
    WHERE NOT EXISTS (SELECT 1 
					  FROM AFC_DataCenter.Account AS acc 
					  WHERE acc.RawID = tmp.RawID 
						AND acc.ParticipantID = ip_ParticipantID);
    
    IF EXISTS (SELECT 1 FROM Temp_Account WHERE RawID IS NULL OR RawID = '' OR RawID = 'null')	THEN
		SET op_ErrorMessage = "RawID is not null or empty";
        
        LEAVE sp;
    END IF;
    
    IF EXISTS (SELECT 1 FROM Temp_AccountNotExisted)	THEN
		SELECT GROUP_CONCAT(RawID)
        INTO op_ErrorMessage
        FROM Temp_AccountNotExisted;
		SET op_ErrorMessage = CONCAT("Not Existed Data: ", op_ErrorMessage);
        
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
    
	UPDATE Temp_Account 
	SET BankInfo = '[{}]'
	WHERE JSON_LENGTH(BankInfo) =  0;
    
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
    
    INSERT INTO Temp_CurrBankInfo(RawID, ParticipantID, BankName, BankAccountID)
    SELECT 	acc.RawID
		, 	acc.ParticipantID
        , 	acc.BankName
        , 	acc.BankAccountID
    FROM AFC_DataCenter.BankAccount AS acc
    WHERE EXISTS (SELECT 1 FROM Temp_BankInfo AS tmp WHERE acc.RawID = tmp.RawID AND acc.ParticipantID = tmp.ParticipantID);
    
    INSERT IGNORE INTO Temp_BankInfoHasChange(RawID, ParticipantID)
    SELECT 	crr.RawID
		,	crr.ParticipantID
	FROM Temp_CurrBankInfo AS crr
		LEFT JOIN Temp_BankInfo AS upd ON	 crr.RawID = upd.RawID 
										AND  crr.ParticipantID = upd.ParticipantID
										AND  crr.BankName = upd.BankName
										AND  crr.BankAccountID = upd.BankAccountID
	WHERE upd.RawID IS NULL;
    
    INSERT IGNORE INTO Temp_BankInfoHasChange(RawID, ParticipantID)
    SELECT 	upd.RawID
		,	upd.ParticipantID
	FROM Temp_CurrBankInfo AS crr
		RIGHT JOIN Temp_BankInfo AS upd  ON	 crr.RawID = upd.RawID 
										AND  crr.ParticipantID = upd.ParticipantID
										AND  crr.BankName = upd.BankName
										AND  crr.BankAccountID = upd.BankAccountID
	WHERE crr.RawID IS NULL AND upd.BankName IS NOT NULL ; 
    
    INSERT IGNORE INTO Temp_AccountNoChange(RawID)
    SELECT tmp.RawID
    FROM Temp_BankInfo AS tmp
    WHERE  NOT EXISTS (SELECT 1 FROM Temp_BankInfoHasChange AS chg WHERE tmp.RawID = chg.RawID AND tmp.ParticipantID = chg.ParticipantID);
    
    INSERT IGNORE INTO Temp_AccountNoChange(RawID)
    SELECT 	tmp.RawID 
	FROM AFC_DataCenter.Account AS acc
		INNER JOIN Temp_Account AS tmp ON acc.ParticipantID = tmp.ParticipantID
									 AND acc.RawID = tmp.RawID
                                     AND tmp.FraudID 	IS NULL
									 AND (tmp.FullName	IS NULL OR acc.FullName = tmp.FullName)
									 AND (tmp.Email 	IS NULL	OR acc.Email = tmp.Email)
									 AND (tmp.Phone 	IS NULL	OR (acc.Phone = tmp.Phone AND acc.CountryCode = tmp.CountryCode))
									 AND (tmp.DocumentID 	IS NULL	OR (acc.DocumentID = tmp.DocumentID AND  acc.DocumentTypeID = tmp.DocumentTypeID))
                                     AND tmp.BankInfo IS NULL;
	
    
    DELETE 
    FROM Temp_Account AS acc
    WHERE EXISTS (SELECT 1 
				  FROM Temp_AccountNoChange AS tmp 
                  WHERE acc.ParticipantID = ip_ParticipantID
					AND acc.RawID = tmp.RawID);
    
    
	INSERT INTO Account_Log(RawID, ParticipantID, FullName, Phone, CountryCode, Email, DocumentID, DocumentTypeID, BankInfo, FraudID, IsUpdated)
    SELECT 	acc.RawID
		, 	acc.ParticipantID
        , 	acc.FullName
        , 	acc.Phone
        , 	acc.CountryCode
        , 	acc.Email
        , 	acc.DocumentID
        , 	acc.DocumentTypeID
        , 	(SELECT JSON_ARRAYAGG(JSON_OBJECT('BankName', bank.BankName,'BankAccountID', bank.BankAccountID)) 
			 FROM BankAccount AS bank 
             WHERE  bank.RawID = acc.RawID 
				AND bank.ParticipantID = acc.ParticipantID) AS BankInfo
        , 	acc.FraudID
        ,	1
	FROM AFC_DataCenter.Account AS acc
    WHERE EXISTS (SELECT 1 FROM Temp_Account AS tmp WHERE tmp.RawID = acc.RawID AND acc.ParticipantID = ip_ParticipantID);
    
    UPDATE AFC_DataCenter.Account AS acc
    INNER JOIN Temp_Account AS tmp ON acc.RawID = tmp.RawID AND acc.ParticipantID = ip_ParticipantID
    SET 	acc.FullName 		= CASE WHEN tmp.FullName 		IS NULL THEN acc.FullName 		ELSE  tmp.FullName END
		,	acc.Phone 			= CASE WHEN tmp.Phone 			IS NULL THEN acc.Phone 			ELSE  tmp.Phone END
        ,	acc.CountryCode 	= CASE WHEN tmp.CountryCode		IS NULL THEN acc.CountryCode 	ELSE  tmp.CountryCode END
        ,	acc.Email			= CASE WHEN tmp.Email 			IS NULL THEN acc.Email 			ELSE  tmp.Email END
        ,	acc.DocumentID 		= CASE WHEN tmp.DocumentID	 	IS NULL THEN acc.DocumentID 	ELSE  tmp.DocumentID END
        ,	acc.DocumentTypeID 	= CASE WHEN tmp.DocumentTypeID 	IS NULL THEN acc.DocumentTypeID ELSE  tmp.DocumentTypeID END
        ,	acc.FraudID 		= CASE WHEN tmp.FraudID 		IS NULL THEN acc.FraudID 		ELSE  tmp.FraudID END;
	
    DELETE acc
    FROM AFC_DataCenter.BankAccount AS acc
    WHERE EXISTS (SELECT 1 FROM Temp_BankInfo AS tmp WHERE acc.RawID = tmp.RawID AND acc.ParticipantID = tmp.ParticipantID);
    
    INSERT IGNORE INTO AFC_DataCenter.BankAccount(RawID,ParticipantID,BankName,BankAccountID)
    SELECT 	tmp.RawID
		,	tmp.ParticipantID
		,	tmp.BankName
        ,	tmp.BankAccountID
    FROM Temp_BankInfo AS tmp
    WHERE tmp.BankName IS NOT NULL; 
	
END$$
DELIMITER ;

