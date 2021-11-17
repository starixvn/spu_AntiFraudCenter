namespace AntiFraudCenterService.Features.FraudCustomer
{
    using AntiFraudCenterService.Features.FraudCustomer.Criteria;
    using AntiFraudCenterService.Features.ParticipantInformation.Model;
    using AntiFraudCenterService.Shares.ViewModel;
    using Fanex.Data.Repository;
    using System.Collections.Generic;
    using System.Linq;
    using static AntiFraudCenterService.Shares.Constants;

    public interface IParticipantInfoDataAccess
    {
        RegenerateKeyResultModel UpdateParticipantSecretKey(int? participantID, string secretKey);

        IEnumerable<FraudCasesMappingModel> GetFraudCases();

        IEnumerable<DocumentMappingModel> GetDocumentTypes();

        InitParticipantResultModel InitParticipant(string participantName, string secretKey);
    }

    public class ParticipantInfoDataAccess : IParticipantInfoDataAccess
    {
        private readonly IDynamicRepository repository;

        public ParticipantInfoDataAccess()
        {
            repository = new DynamicRepository();
        }

        public RegenerateKeyResultModel UpdateParticipantSecretKey(int? participantID, string secretKey)
        {
            var criteria = new ParticipantSecretKeyUpdateCriteria
            {
                ParticipantID = participantID,
                SecretKey = secretKey
            };

            repository.Execute(criteria);

            return new RegenerateKeyResultModel
            {
                StatusCode = HttpStatus.OK,
                ErrorMessage = string.Empty,
                Data = secretKey
            };
        }

        public IEnumerable<FraudCasesMappingModel> GetFraudCases()
        {
            var criteria = new FraudCasesGetCriteria();

            return repository.Fetch<FraudCasesMappingModel>(criteria);
        }

        public IEnumerable<DocumentMappingModel> GetDocumentTypes()
        {
            var criteria = new DocumentTypesGetCriteria();

            return repository.Fetch<DocumentMappingModel>(criteria);
        }

        public InitParticipantResultModel InitParticipant(string participantName, string secretKey)
        {
            var criteria = new ParticipantInitCriteria
            {
                ParticipantName = participantName,
                SecretKey = secretKey
            };

            var result = repository.Fetch<InitParticipantModel>(criteria);

            return new InitParticipantResultModel
            {
                ErrorMessage = criteria.ErrorMessage,
                StatusCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? HttpStatus.OK : HttpStatus.ServerNotImplemented,
                Participant = (result != null && result.Any()) ? result.FirstOrDefault() : new InitParticipantModel()
            };
        }
    }
}