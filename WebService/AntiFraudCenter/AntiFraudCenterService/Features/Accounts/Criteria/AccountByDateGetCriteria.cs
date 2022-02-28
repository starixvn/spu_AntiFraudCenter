namespace AntiFraudCenterService.Features.Accounts.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class AccountByDateGetCriteria : CriteriaBase
    {
        public readonly bool isInternalRequest;

        public AccountByDateGetCriteria(bool isInternalRequest)
        {
            this.isInternalRequest = isInternalRequest;
        }

        [SpParam(SqlParamName = "ip_ParticipantID")]
        public int ParticipantID { get; set; }

        [SpParam(SqlParamName = "ip_ParticipantList")]
        public string ParticipantName { get; set; }

        [SpParam(SqlParamName = "ip_FromDate")]
        public DateTime? FromDate { get; set; }

        [SpParam(SqlParamName = "ip_ToDate")]
        public DateTime? ToDate { get; set; }

        [SpParam(SqlParamName = "op_IsContinue")]
        public int IsContinue { get; set; }

        [SpParam(SqlParamName = "op_FromDate")]
        public DateTime? BatchFromDate { get; set; }

        [SpParam(SqlParamName = "op_ToDate")]
        public DateTime? BatchToDate { get; set; }

        public override string GetSettingKey()
        {
            return isInternalRequest ? "AFC_DC_Account_Scan" : "Ext_AFC_DC_Account_Scan";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}