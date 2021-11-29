namespace AntiFraudCenterService.Features.FraudCustomer.Criteria
{
    using Fanex.Data.Repository;

    public class FraudCasesGetCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "AFC_DC_FraudCase_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}