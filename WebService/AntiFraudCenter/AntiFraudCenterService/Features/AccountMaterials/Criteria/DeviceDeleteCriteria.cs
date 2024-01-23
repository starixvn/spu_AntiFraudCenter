namespace AntiFraudCenterService.Features.AccountMaterials.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class DeviceDeleteCriteria : NonQueryCommand
    {
        public readonly bool isInternalRequest;

        public DeviceDeleteCriteria(bool isInternalRequest)
        {
            this.isInternalRequest = isInternalRequest;
        }

        [SpParam(SqlParamName = "ip_AccountInfo")]
        public string JsonAccountInfo { get; set; }

        [SpParam(SqlParamName = "ip_ParticipantID")]
        public int? ParticipantID { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return isInternalRequest ? "AFC_DC_Account_DeleteDevice" : "Ext_AFC_DC_Account_DeleteDevice";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}