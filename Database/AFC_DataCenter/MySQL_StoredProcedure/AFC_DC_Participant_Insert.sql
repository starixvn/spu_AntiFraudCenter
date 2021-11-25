/*<info serverAlias="AFC-AFC_DataCenter" databaseType="2" executers="afcExtAPIAdmin,afcAPIAdmin" isFunction="0" isNested="0"></info>*/
DROP PROCEDURE IF EXISTS `AFC_DC_Participant_Insert`;

DELIMITER $$
CREATE DEFINER=`afcOwner`@`%` PROCEDURE `AFC_DC_Participant_Insert`(
		IN 	ip_ParticipantName		VARCHAR(100)
    ,	IN 	ip_SecretKey		  	VARCHAR(128)
    ,	IN 	ip_IPs			  		VARCHAR(200)
    
    ,	OUT	op_ErrorMessage			VARCHAR(100)
)
    SQL SECURITY INVOKER
sp :BEGIN 
/* 
		Created:	20211111@Aries.Nguyen  
		Task:		Add Participant
		DB:			AFC_DataCenter
		Original:
		Revisions:
			- 20211111@Aries.Nguyen: Created [Redmine ID: #164149]
            
        Param's Explanation (filtered by):
        
        Example:
			- CALL AFC_DC_Participant_Insert("Participant A", "SecretKeyA", "8.8.8.8,8.8.8.9",@op_ErrorMessage);
	*/   
    IF EXISTS (SELECT 1 FROM AFC_DataCenter.Participant WHERE SecretKey = ip_SecretKey)	THEN
		SET op_ErrorMessage = "SecretKey is exist";
        
        LEAVE sp;
    END IF;
    
    IF EXISTS (SELECT 1 FROM AFC_DataCenter.Participant WHERE ParticipantName = ip_ParticipantName)	THEN
		SET op_ErrorMessage = "Participant name is exist";
        
        LEAVE sp;
    END IF;
    
    INSERT INTO AFC_DataCenter.Participant(ParticipantName,SecretKey,IPs,IsActive)
	VALUES (ip_ParticipantName, ip_SecretKey, ip_IPs, 1);
    
    SELECT 	ParticipantID
		,	ParticipantName
        ,	SecretKey
        ,	IPs
	FROM AFC_DataCenter.Participant
    WHERE SecretKey = ip_SecretKey;
    
END$$
DELIMITER ;

