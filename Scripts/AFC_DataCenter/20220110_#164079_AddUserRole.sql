/*<info serverAlias="AFC-AFC_DataCenter" databaseType="2"></info>*/
/*
	Created: 20220110@Aries.Nguyen [Redmine ID: #164079]
	Task: Add user 
	DB: AFC_DataCenter
*/

CREATE USER 'afcWeb'@'%' IDENTIFIED WITH mysql_native_password BY 'xxx';
GRANT `AFCReader`@`%` TO `afcWeb`@`%`;

CREATE USER 'afcWebAdmin'@'%' IDENTIFIED WITH mysql_native_password BY 'xxx';
GRANT `AFCWriter`@`%` TO `afcWebAdmin`@`%`;

CREATE USER 'afcService'@'%' IDENTIFIED WITH mysql_native_password BY 'xxx';
GRANT `AFCReader`@`%` TO `afcService`@`%`;

CREATE USER 'afcServiceAdmin'@'%' IDENTIFIED WITH mysql_native_password BY 'xxx';
GRANT `AFCWriter`@`%` TO `afcServiceAdmin`@`%`;

FLUSH PRIVILEGES;

