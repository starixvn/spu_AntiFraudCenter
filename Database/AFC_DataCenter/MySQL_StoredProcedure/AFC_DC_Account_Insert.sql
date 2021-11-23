/*<info serverAlias="AFC-AFC_DataCenter" databaseType="2" executers="afcExtAPIAdmin,afcAPIAdmin" isFunction="0" isNested="0"></info>*/
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
            
        Param's Explanation (filtered by):
        
        Example:
			- CALL AFC_DC_Account_Insert(1,'[{"AccountID": "AccountID_Vaule_Test",
											  "FullName": "FullName_Vaule_Test",
                                              "Phone":  "Phone_Vaule_Test",
                                              "CountryCode":"CountryCode_Vaule_Test",
                                              "Email": "Email_Vaule_Test",
                                              "DocumentID": "DocumentID_Vaule_Test",
                                              "DocumentTypeID": 1,
                                              "BankName": "BankName_Vaule_Test",
                                              "BankAccountID": "BankAccountID_Vaule_Test",
                                              "FraudID": 2}]', @mess);
	*/  
    DECLARE lv_CheckValid INT;
    
    DROP TEMPORARY TABLE IF EXISTS Temp_AccountExisted;    
	CREATE TEMPORARY TABLE Temp_AccountExisted( 	  
			RawID 	VARCHAR(128) PRIMARY KEY
	);
    
	DROP TEMPORARY TABLE IF EXISTS Temp_Account;    
	CREATE TEMPORARY TABLE Temp_Account( 	  
			RawID 			VARCHAR(128)
		,	ParticipantID	INT
		,	FullName 		NVARCHAR(128)
		,	Phone 			VARCHAR(128)
		,	CountryCode 	VARCHAR(8)
		,	Email 			VARCHAR(128)
		,	DocumentID  	VARCHAR(128)
		,	DocumentTypeID  INT
		,	BankName		VARCHAR(128)
		,	BankAccountID	VARCHAR(128)
		,	FraudID			INT
	);
    
	INSERT IGNORE INTO Temp_Account(RawID, ParticipantID, FullName, Phone, CountryCode, Email, DocumentID, DocumentTypeID, BankName, BankAccountID, FraudID)
	SELECT	RawID
		, 	ip_ParticipantID
		, 	FullName
		, 	Phone
        , 	CountryCode
        , 	Email
        , 	DocumentID
        , 	DocumentTypeID
        , 	BankName
        , 	BankAccountID
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
			,	BankName		VARCHAR(128)	PATH "$.BankName" 
			,	BankAccountID	VARCHAR(128)	PATH "$.BankAccountID" 
			,	FraudID			INT				PATH "$.FraudID" 
		)
	) AS info;
    
    INSERT IGNORE INTO Temp_AccountExisted(RawID)
    SELECT tmp.RawID 
    FROM  Temp_Account AS tmp 
    WHERE EXISTS (SELECT 1 
				  FROM AFC_DataCenter.Account AS acc 
				  WHERE acc.RawID = tmp.RawID 
					AND acc.ParticipantID = ip_ParticipantID);
	
    IF EXISTS (SELECT 1 FROM Temp_Account WHERE RawID IS NULL OR RawID = '')	THEN
		SET op_ErrorMessage = "RawID isn't null or empty";
        
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
    FROM Temp_Account AS tmp 
	WHERE NOT EXISTS (SELECT 1 FROM FraudCase AS fr WHERE fr.FraudID = tmp.FraudID)
    LIMIT 1;
        
	IF lv_CheckValid = 1 THEN
		SET op_ErrorMessage = "FraudID invalid";
        
        LEAVE sp;
    END IF;
    
    
    
    
    INSERT INTO AFC_DataCenter.Account(RawID, ParticipantID, FullName, Phone, CountryCode, Email, DocumentID, DocumentTypeID, BankName, BankAccountID, FraudID)
    SELECT 	RawID
		, 	ParticipantID
		, 	FullName
		, 	Phone
        , 	CountryCode
        , 	Email
        , 	DocumentID
        , 	DocumentTypeID
        , 	BankName
        , 	BankAccountID
        , 	FraudID
    FROM Temp_Account;
END$$
DELIMITER ;

