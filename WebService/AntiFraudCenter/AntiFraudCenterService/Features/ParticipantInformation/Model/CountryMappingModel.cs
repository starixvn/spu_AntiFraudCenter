namespace AntiFraudCenterService.Features.ParticipantInformation.Model
{
    using Newtonsoft.Json;

    public class CountryMappingModel
    {
        [JsonProperty("CountryName")]
        public string CountryName { get; set; }

        [JsonProperty("CountryCode")]
        public string CountryCode { get; set; }
    }
}
