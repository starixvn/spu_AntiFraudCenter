namespace AntiFraudCenterService.Features.FraudCustomer
{
    using AntiFraudCenterService.Features.ParticipantInformation.Model;
    using AntiFraudCenterService.Shares.ViewModel;
    using System;
    using System.Collections.Generic;
    using static AntiFraudCenterService.Shares.Constants;

    public interface IParticipantInfoDBService
    {
        RegenerateKeyResultModel UpdateParticipantSecretKey(int? participantID, string newSecretKey);

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

        public RegenerateKeyResultModel UpdateParticipantSecretKey(int? participantID, string newSecretKey)
        {
            if (!string.IsNullOrEmpty(newSecretKey) && participantID != null)
            {
                try
                {
                    return participantInfoDataAccess.UpdateParticipantSecretKey(participantID, newSecretKey);
                }
                catch(Exception ex)
                {
                    return new RegenerateKeyResultModel() { StatusCode = HttpStatus.ServerNotImplemented, ErrorMessage = ex.Message };
                }
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