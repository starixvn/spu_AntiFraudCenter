namespace AntiFraudCenterService.Features.Participants.Model
{
    using Newtonsoft.Json;
    using static AntiFraudCenterService.Shares.Constants;

    public class InitParticipantResultModel
    {
        [JsonProperty(PropertyName = "StatusCode")]
        public HttpStatus StatusCode { get; set; }

        [JsonProperty(PropertyName = "ErrorMessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty(PropertyName = "Participant")]
        public InitParticipantModel Participant { get; set; }
    }
}