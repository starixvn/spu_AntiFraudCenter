namespace AntiFraudCenterService.Features.ParticipantInformation.Model
{
    using System.Collections.Generic;

    public class ConventionViewModel
    {
        public IEnumerable<FraudCasesModel> FraudCases { get; set; }

        public IEnumerable<DocumentModel> DocumentTypes { get; set; }
    }
}
