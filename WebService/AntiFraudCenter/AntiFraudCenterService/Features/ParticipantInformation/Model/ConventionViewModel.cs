namespace AntiFraudCenterService.Features.ParticipantInformation.Model
{
    using System.Collections.Generic;
    using static AntiFraudCenterService.Shares.Constants;

    public class ConventionViewModel
    {
        public IEnumerable<FraudCasesModel> FraudCases { get; set; }

        public IEnumerable<DocumentModel> DocumentTypes { get; set; }

        public HttpStatus StatusCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}
