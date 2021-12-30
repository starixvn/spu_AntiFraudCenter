namespace AntiFraudCenterService.Features.Participants.Model
{
    using Newtonsoft.Json;
    using static AntiFraudCenterService.Shares.Constants;

    public class RegenerateKeyResultModel
    {
        [JsonProperty(PropertyName = "StatusCode")]
        public HttpStatus StatusCode { get; set; }

        [JsonProperty(PropertyName = "ErrorMessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty(PropertyName = "Data")]
        public string Data { get; set; }
    }
}