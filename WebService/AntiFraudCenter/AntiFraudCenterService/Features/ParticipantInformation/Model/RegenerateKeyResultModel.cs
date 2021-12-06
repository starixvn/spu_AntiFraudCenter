namespace AntiFraudCenterService.Shares.ViewModel
{
    using Newtonsoft.Json;
    using static AntiFraudCenterService.Shares.Constants;

    public class RegenerateKeyResultModel
    {
        [JsonProperty(PropertyName = "StatusCode")]
        public HttpStatus StatusCode { get; set; }

        [JsonProperty(PropertyName = "StatusCode")]
        public string ErrorMessage { get; set; }

        [JsonProperty(PropertyName = "StatusCode")]
        public string Data { get; set; }
    }
}