namespace AntiFraudCenterService.Features.Accounts.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class AccountUpdateCriteria : NonQueryCommand
    {
        public readonly bool isInternalRequest;

        public AccountUpdateCriteria(bool isInternalRequest)
        {
            this.isInternalRequest = isInternalRequest;
        }

        [SpParam(SqlParamName = "ip_AccountInfo")]
        public string JsonAccountInfo { get; set; }

        [SpParam(SqlParamName = "ip_ParticipantID")]
        public int? ParticipantID { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return isInternalRequest ? "AFC_DC_Account_Update" : "Ext_AFC_DC_Account_Update";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}