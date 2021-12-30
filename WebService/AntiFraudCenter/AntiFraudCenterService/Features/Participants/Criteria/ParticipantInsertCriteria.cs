namespace AntiFraudCenterService.Features.Participants.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ParticipantInsertCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_ParticipantName")]
        public string ParticipantName { get; set; }

        [SpParam(SqlParamName = "ip_SecretKey")]
        public string SecretKey { get; set; }

        [SpParam(SqlParamName = "ip_IPs")]
        public string IPs { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "AFC_DC_Participant_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}