namespace AntiFraudCenterService.Features.FraudCustomer.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class AccountDeleteCriteria : NonQueryCommand
    {
        public readonly bool isInternalRequest;

        public AccountDeleteCriteria(bool isInternalRequest)
        {
            this.isInternalRequest = isInternalRequest;
        }

        [SpParam(SqlParamName = "ip_AccountInfo")]
        public string JsonAccountInfo { get; set; }

        [SpParam(SqlParamName = "ip_ParticipantID")]
        public int? ParticipantID { get; set; }

        public override string GetSettingKey()
        {
            return isInternalRequest ? "AFC_DC_Account_Delete" : "Ext_AFC_DC_Account_Delete";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}