namespace AntiFraudCenterService.Features.Participants
{
    using AntiFraudCenterService.Features.Participants.Criteria;
    using AntiFraudCenterService.Features.Participants.Model;
    using Fanex.Data.Repository;
    using System.Collections.Generic;
    using System.Linq;
    using static AntiFraudCenterService.Shares.Constants;

    public interface IParticipantInfoDataAccess
    {
        RegenerateKeyResultModel UpdateParticipantSecretKey(int? participantID, string secretKey, bool isInternalRequest);

        IEnumerable<FraudCasesMappingModel> GetFraudCases();

        IEnumerable<DocumentMappingModel> GetDocumentTypes();

        InitParticipantResultModel InitParticipant(string participantName, string secretKey, string ips);
    }

    public class ParticipantInfoDataAccess : IParticipantInfoDataAccess
    {
        private readonly IDynamicRepository repository;

        public ParticipantInfoDataAccess()
        {
            repository = new DynamicRepository();
        }

        public RegenerateKeyResultModel UpdateParticipantSecretKey(int? participantID, string secretKey, bool isInternalRequest)
        {
            var criteria = new ParticipantSecretKeyUpdateCriteria(isInternalRequest)
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

        public InitParticipantResultModel InitParticipant(string participantName, string secretKey, string ips)
        {
            var criteria = new ParticipantInsertCriteria
            {
                ParticipantName = participantName,
                SecretKey = secretKey,
                IPs = ips
            };

            var result = repository.Fetch<InitParticipantModel>(criteria);

            return new InitParticipantResultModel
            {
                ErrorMessage = criteria.ErrorMessage,
                StatusCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? HttpStatus.OK : HttpStatus.BadRequest,
                Participant = (result != null && result.Any()) ? result.FirstOrDefault() : new InitParticipantModel()
            };
        }
    }
}