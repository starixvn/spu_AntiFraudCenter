/*<info serverAlias="AFCMain-AFC_DataCenter" databaseType="2" executers="afcExtAPIAdmin,afcAPIAdmin" isFunction="0" isNested="0"></info>*/
DROP PROCEDURE IF EXISTS `AFC_DC_Participant_UpdateSecretKey`;

DELIMITER $$
CREATE DEFINER=`afcOwner`@`%` PROCEDURE `AFC_DC_Participant_UpdateSecretKey`(
		IN	ip_ParticipantID	INT
	,	IN	ip_SecretKey 		VARCHAR(128)
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
			- CALL AFC_DC_Participant_UpdateSecretKey('1', 'SecretKey');
	*/   
    INSERT INTO Participant_Log(ParticipantID, SecretKey, IsUpdated)
    SELECT 	ParticipantID
		, 	SecretKey
        , 	1
    FROM AFC_DataCenter.Participant
    WHERE ParticipantID = ip_ParticipantID;
    
	UPDATE AFC_DataCenter.Participant
    SET SecretKey = ip_SecretKey
	WHERE  ParticipantID = ip_ParticipantID;
    
END$$
DELIMITER ;

