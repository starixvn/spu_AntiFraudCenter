namespace AntiFraudCenterService.Features.FraudCustomer.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class AccountGetCriteria : CriteriaBase
    {
        public readonly bool isInternalRequest;

        public AccountGetCriteria(bool isInternalRequest)
        {
            this.isInternalRequest = isInternalRequest;
        }

        [SpParam(SqlParamName = "ip_AccountInfo")]
        public string JsonAccountInfo { get; set; }

        public override string GetSettingKey()
        {
            return isInternalRequest ? "AFC_DC_Account_Get" : "Ext_AFC_DC_Account_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}