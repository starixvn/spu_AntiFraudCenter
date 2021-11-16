namespace AntiFraudCenterService.Features.FraudCustomer
{
    using AntiFraudCenterService.Features.ParticipantInformation.Model;
    using AntiFraudCenterService.Shares.Model;
    using AntiFraudCenterService.Shares.ViewModel;
    using System.Collections.Generic;
    using static AntiFraudCenterService.Shares.Constants;

    public interface IParticipantInfoDBService
    {
        RegenerateKeyResultModel UpdateParticipantSecretKey(int? participantID, string newSecretKey, string enscryptedNewKey);

        IEnumerable<FraudCasesModel> GetFraudCases();

        IEnumerable<DocumentModel> GetDocumentTypes();
    }

    public class ParticipantInfoDBService : IParticipantInfoDBService
    {
        private readonly IParticipantInfoDataAccess participantInfoDataAccess;

        public ParticipantInfoDBService(IParticipantInfoDataAccess participantInfoDataAccess)
        {
            this.participantInfoDataAccess = participantInfoDataAccess;
        }

        public RegenerateKeyResultModel UpdateParticipantSecretKey(int? participantID, string newSecretKey, string enscryptedNewKey)
        {
            if (!string.IsNullOrEmpty(newSecretKey) && participantID != null)
            {
                var rs = participantInfoDataAccess.UpdateParticipantSecretKey(participantID, newSecretKey);

                return new RegenerateKeyResultModel
                {
                    StatusCode = rs.StatusCode,
                    ErrorMessage = rs.ErrorMessage,
                    EncryptedData = rs.StatusCode == HttpStatus.OK ? enscryptedNewKey : string.Empty
                };
            }

            return new RegenerateKeyResultModel() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
        }

        public IEnumerable<FraudCasesModel> GetFraudCases()
        {
            return participantInfoDataAccess.GetFraudCases();
        }

        public IEnumerable<DocumentModel> GetDocumentTypes()
        {
            return participantInfoDataAccess.GetDocumentTypes();
        }

    }
}