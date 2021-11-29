namespace AntiFraudCenterService.Features.FraudCustomer.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class AccountGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_AccountInfo")]
        public string JsonAccountInfo { get; set; }

        public override string GetSettingKey()
        {
            return "AFC_DC_Account_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}