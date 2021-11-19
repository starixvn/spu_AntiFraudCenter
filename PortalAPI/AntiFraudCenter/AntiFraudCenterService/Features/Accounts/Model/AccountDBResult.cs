namespace AntiFraudCenterService.Shares.Model
{
    using static AntiFraudCenterService.Shares.Constants;

    public class AccountDBResult
    {
        public HttpStatus StatusCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}