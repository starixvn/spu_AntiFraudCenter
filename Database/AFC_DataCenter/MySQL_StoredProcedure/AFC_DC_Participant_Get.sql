/*<info serverAlias="AFC-AFC_DataCenter" databaseType="2" executers="afcExtAPI,afcAPI" isFunction="0" isNested="0"></info>*/
DROP PROCEDURE IF EXISTS `AFC_DC_Participant_Get`;

DELIMITER $$
CREATE DEFINER=`afcOwner`@`%` PROCEDURE `AFC_DC_Participant_Get`()
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
			- CALL AFC_DC_Participant_GetAll();
	*/   
	SELECT 	ParticipantID
		,	ParticipantName
        ,	SecretKey
	FROM AFC_DataCenter.Participant
    WHERE IsActive = 1;
    
END$$
DELIMITER ;

