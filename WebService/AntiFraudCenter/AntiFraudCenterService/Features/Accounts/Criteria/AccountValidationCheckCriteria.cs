namespace AntiFraudCenterService.Features.Accounts.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class AccountValidationCheckCriteria : CriteriaBase
    {
        public readonly bool isInternalRequest;

        public AccountValidationCheckCriteria(bool isInternalRequest)
        {
            this.isInternalRequest = isInternalRequest;
        }

        [SpParam(SqlParamName = "ip_ParticipantID")]
        public int? ParticipantID { get; set; }

        [SpParam(SqlParamName = "ip_AccountInfo")]
        public string JsonAccountInfo { get; set; }

        public override string GetSettingKey()
        {
            return isInternalRequest ? "AFC_DC_Account_CheckExist" : "Ext_AFC_DC_Account_CheckExist";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}