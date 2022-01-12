/*<info serverAlias="AFC-AFC_DataCenter" databaseType="2" executers="afcExtAPI,afcAPI,afcWeb,afcService" isFunction="0" isNested="0"></info>*/
DROP PROCEDURE IF EXISTS `AFC_DC_FraudCase_Get`;

DELIMITER $$
CREATE DEFINER=`afcOwner`@`%` PROCEDURE `AFC_DC_FraudCase_Get`()
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
			- CALL AFC_DC_FraudCase_Get();
	*/   
	SELECT 	FraudID
		,	FraudName
	FROM AFC_DataCenter.FraudCase;
    
END$$
DELIMITER ;

