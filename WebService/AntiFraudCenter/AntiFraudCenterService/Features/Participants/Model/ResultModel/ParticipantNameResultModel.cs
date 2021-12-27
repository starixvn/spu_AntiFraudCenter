namespace AntiFraudCenterService.Features.Participants.Model
{
    using Newtonsoft.Json;

    public class ParticipantNameResultModel
    {
        [JsonProperty(PropertyName = "ParticipantName")]
        public string ParticipantName { get; set; }
    }
}