namespace AntiFraudCenterService.Features.Accounts.Criteria
{
    using Fanex.Data.Repository;

    public class ParticipantGetCriteria : CriteriaBase
    {
        public readonly bool isInternalRequest;

        public ParticipantGetCriteria(bool isInternalRequest)
        {
            this.isInternalRequest = isInternalRequest;
        }

        public override string GetSettingKey()
        {
            return isInternalRequest ? "AFC_DC_Participant_Get" : "Ext_AFC_DC_Participant_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}