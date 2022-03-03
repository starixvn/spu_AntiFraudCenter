/*<info serverAlias="AFC-AFC_DataCenter" databaseType="2"></info>*/
/*
	Created: 20220110@Aries.Nguyen [Redmine ID: #164079]
	Task: Multiple bank account
	DB: AFC_DataCenter
*/
USE AFC_DataCenter;
ALTER DATABASE AFC_DataCenter CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci;

ALTER TABLE AFC_DataCenter.Account CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE AFC_DataCenter.Account_Log CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE AFC_DataCenter.BankAccount CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE AFC_DataCenter.DocumentType CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE AFC_DataCenter.ErrorLog CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE AFC_DataCenter.FraudCase CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE AFC_DataCenter.Participant CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE AFC_DataCenter.Participant_Log CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE AFC_DataCenter.SystemParameter CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
