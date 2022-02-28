namespace AntiFraudCenterService.Features.Accounts.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ErrorLogInsertCriteria : NonQueryCommand
    {
        public readonly bool isInternalRequest;

        public ErrorLogInsertCriteria(bool isInternalRequest)
        {
            this.isInternalRequest = isInternalRequest;
        }

        [SpParam(SqlParamName = "ip_ParticipantID")]
        public int? ParticipantID { get; set; }

        [SpParam(SqlParamName = "ip_Message")]
        public string ErrorMessage { get; set; }

        [SpParam(SqlParamName = "ip_Info")]
        public string DataJson { get; set; }

        public override string GetSettingKey()
        {
            return isInternalRequest ? "AFC_DC_ErrorLog_Insert" : "Ext_AFC_DC_ErrorLog_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}