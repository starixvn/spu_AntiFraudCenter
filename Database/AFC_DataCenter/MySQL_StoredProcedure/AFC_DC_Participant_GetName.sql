/*<info serverAlias="AFC-AFC_DataCenter" databaseType="2" executers="afcExtAPI,afcAPI" isFunction="0" isNested="0"></info>*/
DROP PROCEDURE IF EXISTS `AFC_DC_Participant_GetName`;

DELIMITER $$
CREATE DEFINER=`afcOwner`@`%` PROCEDURE `AFC_DC_Participant_GetName`()
    SQL SECURITY INVOKER
BEGIN 
/* 
		Created:	20211227@Aries.Nguyen
		Task:		Scanning Account
		DB:			AFC_DataCenter
		Original:
		Revisions:   
			- 20211227@Aries.Nguyen: Created [Redmine ID: #164149]
            
        Param's Explanation (filtered by):
        
        Example:
			- CALL AFC_DC_Participant_GetName();
	*/   
	SELECT 	ParticipantName
	FROM AFC_DataCenter.Participant
	WHERE IsActive = 1;
    
END$$
DELIMITER ;

