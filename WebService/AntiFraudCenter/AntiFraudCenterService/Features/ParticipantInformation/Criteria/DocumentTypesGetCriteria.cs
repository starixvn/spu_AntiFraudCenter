namespace AntiFraudCenterService.Features.FraudCustomer.Criteria
{
    using Fanex.Data.Repository;

    public class DocumentTypesGetCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "AFC_DC_DocumentType_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}