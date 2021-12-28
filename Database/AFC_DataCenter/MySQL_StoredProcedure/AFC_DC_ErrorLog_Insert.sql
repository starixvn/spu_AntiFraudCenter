/*<info serverAlias="AFC-AFC_DataCenter" databaseType="2" executers="afcExtAPIAdmin,afcAPIAdmin" isFunction="0" isNested="0"></info>*/
DROP PROCEDURE IF EXISTS `AFC_DC_Participant_UpdateSecretKey`;

DELIMITER $$
CREATE DEFINER=`afcOwner`@`%` PROCEDURE `AFC_DC_ErrorLog_Insert`(
		IN 	ip_ParticipantID	INT UNSIGNED
	,	IN 	ip_Message			VARCHAR(300)
    ,	IN 	ip_Info				JSON
    
)
    SQL SECURITY INVOKER
sp :BEGIN 
/* 
		Created:	20211228@Aries.Nguyen  
		Task:		Write log
		DB:			AFC_DataCenter
		Original:
		Revisions:
			- 20211228@Aries.Nguyen: Created [Redmine ID: #164149]
            
        Param's Explanation (filtered by):
        
        Example:
			- CALL AFC_DC_ErrorLog_Insert(1, 'Error message', '{"accountID":1}');
	*/   
    INSERT INTO AFC_DataCenter.ErrorLog(ParticipantID, Message, Info) 
	VALUES (ip_ParticipantID, ip_Message, ip_Info);
END$$
DELIMITER ;

