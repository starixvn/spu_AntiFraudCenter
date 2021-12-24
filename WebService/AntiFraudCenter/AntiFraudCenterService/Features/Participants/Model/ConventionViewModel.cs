namespace AntiFraudCenterService.Features.Participants.Model
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using static AntiFraudCenterService.Shares.Constants;

    public class ConventionViewModel
    {
        [JsonProperty(PropertyName = "FraudCasesMapping")]
        public IEnumerable<FraudCasesMappingModel> FraudCasesMapping { get; set; }

        [JsonProperty(PropertyName = "DocumentMapping")]
        public IEnumerable<DocumentMappingModel> DocumentMapping { get; set; }

        [JsonProperty(PropertyName = "CountryMapping")]
        public IEnumerable<CountryMappingModel> CountryMapping { get; set; }

        [JsonProperty(PropertyName = "StatusCode")]
        public HttpStatus StatusCode { get; set; }

        [JsonProperty(PropertyName = "ErrorMessage")]
        public string ErrorMessage { get; set; }
    }
}