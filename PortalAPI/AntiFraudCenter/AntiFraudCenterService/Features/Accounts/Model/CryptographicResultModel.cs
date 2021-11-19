namespace AntiFraudCenterService.Shares.Model
{
    using static AntiFraudCenterService.Shares.Constants;

    public class CryptographicResultModel
    {
        public HttpStatus ErrorCode { get; set; }

        public string ErrorMsg { get; set; }

        public string Data { get; set; }
    }
}