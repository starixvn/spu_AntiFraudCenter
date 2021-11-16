namespace AntiFraudCenterService.Features.ParticipantInformation.Model
{
    using System.Collections.Generic;
    using static AntiFraudCenterService.Shares.Constants;

    public class ConventionViewModel
    {
        public IEnumerable<FraudCasesMappingModel> FraudCasesMapping { get; set; }

        public IEnumerable<DocumentMappingModel> DocumentMapping { get; set; }

        public IEnumerable<CountryMappingModel> CountryMapping { get; set; }

        public HttpStatus StatusCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}
