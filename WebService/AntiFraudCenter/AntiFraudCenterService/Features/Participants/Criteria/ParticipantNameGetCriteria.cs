namespace AntiFraudCenterService.Features.Participants.Criteria
{
    using Fanex.Data.Repository;

    public class ParticipantNameGetCriteria : CriteriaBase
    {
        public readonly bool isInternalRequest;

        public ParticipantNameGetCriteria(bool isInternalRequest)
        {
            this.isInternalRequest = isInternalRequest;
        }

        public override string GetSettingKey()
        {
            return isInternalRequest ? "AFC_DC_Participant_GetName" : "Ext_AFC_DC_Participant_GetName";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}