namespace AntiFraudCenter
{
    using AntiFraudCenterService.Features.FraudCustomer;
    using AntiFraudCenterService.Features.ParticipantInformation.Model;
    using AntiFraudCenterService.Shares;
    using AntiFraudCenterService.Shares.Model;
    using AntiFraudCenterService.Shares.ViewModel;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using static AntiFraudCenterService.Shares.Constants;

    public interface IParticipantService
    {
        DBResult UpdateParticipantSecretKey(int? participantID, string encryptedSecretKey);

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

        public DBResult UpdateParticipantSecretKey(int? participantID, string encryptedSecretKey)
        {
            var rs = new DBResult() { ErrorCode = HttpStatus.BadRequest, ErrorMsg = "Invalid input data" };

            try
            {
                if (participantID != null && !string.IsNullOrEmpty(encryptedSecretKey))
                {
                    var participant = fraudCustomerDBService.GetParticipant(participantID);

                    if (participant != null)
                    {
                        var decryptedSecketKey = CryptographicHelper.AESDecrypt(encryptedSecretKey, participant.SecretKey, participant.ParticipantName);

                        if (string.Equals(decryptedSecketKey, participant.SecretKey))
                        {
                            var newSecretKey = CryptographicHelper.GetRandomHexNumber();

                            rs = participantInfoDBService.UpdateParticipantSecretKey(participantID, newSecretKey);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - DeleteFraudCustomer", string.Empty, (participantID, encryptedSecretKey), ex, sentryClient);

                rs.ErrorCode = HttpStatus.BadRequest;
                rs.ErrorMsg = ex.Message;
            }

            return rs;
        }

        public ConventionViewModel GetConventionInfo()
        {
            var rs = new ConventionViewModel { DocumentTypes = new List<DocumentModel>(), FraudCases = new List<FraudCasesModel>()};
            try
            {
                rs.DocumentTypes = participantInfoDBService.GetDocumentTypes();
                rs.FraudCases = participantInfoDBService.GetFraudCases();
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - GetConventionInfo", string.Empty, string.Empty, ex, sentryClient);
            }

            return rs;
        }
    }
}