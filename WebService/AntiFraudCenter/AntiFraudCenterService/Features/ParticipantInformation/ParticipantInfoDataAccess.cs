namespace AntiFraudCenterService.Features.FraudCustomer
{
    using AntiFraudCenterService.Features.FraudCustomer.Criteria;
    using AntiFraudCenterService.Features.ParticipantInformation.Model;
    using AntiFraudCenterService.Shares.Model;
    using Fanex.Data.Repository;
    using System.Collections.Generic;
    using static AntiFraudCenterService.Shares.Constants;

    public interface IParticipantInfoDataAccess
    {
        FraudCustDBResult UpdateParticipantSecretKey(int? participantID, string secretKey);

        IEnumerable<FraudCasesModel> GetFraudCases();

        IEnumerable<DocumentModel> GetDocumentTypes();
    }

    public class ParticipantInfoDataAccess : IParticipantInfoDataAccess
    {
        private readonly IDynamicRepository repository;

        public ParticipantInfoDataAccess()
        {
            repository = new DynamicRepository();
        }

        public FraudCustDBResult UpdateParticipantSecretKey(int? participantID, string secretKey)
        {
            var criteria = new ParticipantSecretKeyUpdateCriteria
            {
                ParticipantID = participantID,
                SecretKey = secretKey
            };

            repository.Execute(criteria);

            return new FraudCustDBResult
            {
                StatusCode = HttpStatus.OK,
                ErrorMessage = string.Empty
            };
        }

        public IEnumerable<FraudCasesModel> GetFraudCases()
        {
            var criteria = new FraudCasesGetCriteria();

            return repository.Fetch<FraudCasesModel>(criteria);
        }

        public IEnumerable<DocumentModel> GetDocumentTypes()
        {
            var criteria = new DocumentTypesGetCriteria();

            return repository.Fetch<DocumentModel>(criteria);
        }
    }
}