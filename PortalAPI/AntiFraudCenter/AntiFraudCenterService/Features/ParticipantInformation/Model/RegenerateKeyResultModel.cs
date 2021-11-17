namespace AntiFraudCenterService.Shares.ViewModel
{
    using static AntiFraudCenterService.Shares.Constants;

    public class RegenerateKeyResultModel
    {
        public HttpStatus StatusCode { get; set; }

        public string ErrorMessage { get; set; }

        public string Data { get; set; }
    }
}