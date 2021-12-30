namespace AntiFraudCenterService.Shares.Model
{
    using Newtonsoft.Json;
    using static AntiFraudCenterService.Shares.Constants;

    public class AccountDBResult
    {
        [JsonProperty(PropertyName = "StatusCode")]
        public HttpStatus StatusCode { get; set; }

        [JsonProperty(PropertyName = "ErrorMessage")]
        public string ErrorMessage { get; set; }
    }
}