namespace AntiFraudCenterService.Features.FraudCustomer.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ParticipantSecretKeyUpdateCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_ParticipantID")]
        public int? ParticipantID { get; set; }

        [SpParam(SqlParamName = "ip_SecretKey")]
        public string SecretKey { get; set; }

        public override string GetSettingKey()
        {
            return "AFC_DC_Participant_UpdateSecretKey";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}