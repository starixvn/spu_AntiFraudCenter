namespace AntiFraudCenter
{
    using AntiFraudCenterService.Features.FraudCustomer;
    using AntiFraudCenterService.Features.ParticipantInformation.Model;
    using AntiFraudCenterService.Shares;
    using AntiFraudCenterService.Shares.ViewModel;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using static AntiFraudCenterService.Shares.Constants;

    public interface IParticipantService
    {
        RegenerateKeyResultModel UpdateParticipantSecretKey(int? participantID, string encryptedSecretKey);

        ConventionViewModel GetConventionInfo();
    }

    public class ParticipantService : IParticipantService
    {
        private readonly ISentryClient sentryClient;
        private readonly IParticipantInfoDBService participantInfoDBService;
        private readonly IFraudCustomerDBService fraudCustomerDBService;
        private readonly IAppSettings appSettings;

        public ParticipantService(
            ISentryClient sentryClient,
            IParticipantInfoDBService participantInfoDBService,
                IFraudCustomerDBService fraudCustomerDBService,
        IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.participantInfoDBService = participantInfoDBService;
            this.fraudCustomerDBService = fraudCustomerDBService;
            this.appSettings = appSettings;
        }

        public RegenerateKeyResultModel UpdateParticipantSecretKey(int? participantID, string encryptedSecretKey)
        {
            var rs = new RegenerateKeyResultModel() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };

            try
            {
                if (string.IsNullOrEmpty(encryptedSecretKey) || participantID == null)
                {
                    return rs;
                }

                var participant = fraudCustomerDBService.GetParticipant(participantID);

                if (participant != null)
                {
                    var decryptedSecketKey = CryptographicHelper.AESDecrypt(encryptedSecretKey, participant.SecretKey, participant.ParticipantName);

                    if (string.Equals(decryptedSecketKey, participant.SecretKey))
                    {
                        var newSecretKey = CryptographicHelper.GetRandomHexNumber();

                        var enscryptedNewKey = CryptographicHelper.AESEncrypt(newSecretKey, participant.SecretKey, participant.ParticipantName);

                        rs = participantInfoDBService.UpdateParticipantSecretKey(participantID, newSecretKey);

                        rs.EncryptedData = enscryptedNewKey;
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - UpdateParticipantSecretKey", string.Empty, (participantID, encryptedSecretKey), ex, sentryClient);

                rs.ErrorMessage = ex.Message;
            }

            return rs;
        }

        public ConventionViewModel GetConventionInfo()
        {
            var rs = new ConventionViewModel { 
                DocumentTypes = new List<DocumentModel>(), FraudCases = new List<FraudCasesModel>(),
                StatusCode = HttpStatus.OK, ErrorMessage = string.Empty
            };

            try
            {
                rs.DocumentTypes = participantInfoDBService.GetDocumentTypes();
                rs.FraudCases = participantInfoDBService.GetFraudCases();
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - GetConventionInfo", string.Empty, string.Empty, ex, sentryClient);
                rs.ErrorMessage = ex.Message;
                rs.StatusCode = HttpStatus.ServerNotImplemented;
            }

            return rs;
        }
    }
}