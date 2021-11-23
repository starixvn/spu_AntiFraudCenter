/*<info serverAlias="AFC-AFC_DataCenter" databaseType="2" executers="afcExtAPIAdmin,afcAPIAdmin" isFunction="0" isNested="0"></info>*/
DROP PROCEDURE IF EXISTS `AFC_DC_Account_Delete`;

DELIMITER $$
CREATE DEFINER=`afcOwner`@`%` PROCEDURE `AFC_DC_Account_Delete`(
		IN	ip_ParticipantID	INT
	,	IN	ip_AccountInfo 		JSON
)
    SQL SECURITY INVOKER
BEGIN
/* 
		Created:	20211111@Aries.Nguyen
		Task:		Add account
		DB:			AFC_DataCenter
		Original:
		Revisions:
			- 20211111@Aries.Nguyen: Created [Redmine ID: #164149]
              
        Param's Explanation (filtered by):
        
        Example:
			- CALL AFC_DC_Account_Delete(1,'[{"AccountID": "AccountID_Vaule_Test"}]');;
	*/   
	DROP TEMPORARY TABLE IF EXISTS Temp_RawID;    
	CREATE TEMPORARY TABLE Temp_RawID( 	  
			RawID 	VARCHAR(128) PRIMARY KEY
	);
    
    INSERT IGNORE INTO Temp_RawID(RawID)
	SELECT	RawID
	FROM JSON_TABLE(ip_AccountInfo,
		 "$[*]" COLUMNS(
				RawID 			VARCHAR(128)		PATH "$.AccountID" 
		)
	) AS info;
    
    INSERT INTO Account_Log(RawID, ParticipantID, FullName, Phone, CountryCode, Email, DocumentID, DocumentTypeID, BankName, BankAccountID, FraudID, IsDeleted)
    SELECT 	acc.RawID
		, 	acc.ParticipantID
        , 	acc.FullName
        , 	acc.Phone
        , 	acc.CountryCode
        , 	acc.Email
        , 	acc.DocumentID
        , 	acc.DocumentTypeID
        , 	acc.BankName
        , 	acc.BankAccountID
        , 	acc.FraudID
        ,	1
	FROM AFC_DataCenter.Account AS acc
    WHERE EXISTS (SELECT 1 FROM Temp_RawID AS tmp WHERE tmp.RawID = acc.RawID AND acc.ParticipantID = ip_ParticipantID);
    
    DELETE 
    FROM AFC_DataCenter.Account AS acc
    WHERE EXISTS (SELECT 1 FROM Temp_RawID AS tmp WHERE tmp.RawID = acc.RawID AND acc.ParticipantID = ip_ParticipantID);
    
END$$
DELIMITER ;

