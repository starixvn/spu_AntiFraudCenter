namespace AntiFraudCenter
{
    using AntiFraudCenterService.Features.FraudCustomer;
    using AntiFraudCenterService.Shares;
    using AntiFraudCenterService.Shares.ViewModel;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using static AntiFraudCenterService.Shares.Constants;

    public interface IFraudCustomerService
    {
        FraudCustResult GetFraudCustomer(int? participantID, string encryptedData);

        string GetFraudCustomerDecryption(int? participantID);
    }

    public class FraudCustomerService : IFraudCustomerService
    {
        private readonly ISentryClient sentryClient;
        private readonly IFraudCustomerDBService fraudCustomerDBService;
        private readonly IAppSettings appSettings;

        public FraudCustomerService(
            ISentryClient sentryClient,
            IFraudCustomerDBService fraudCustomerDBService,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.fraudCustomerDBService = fraudCustomerDBService;
            this.appSettings = appSettings;
        }

        public FraudCustResult GetFraudCustomer(int? participantID, string encryptedData)
        {
            var rs = new FraudCustResult() { ErrorCode = HttpStatus.OK, ErrorMsg = string.Empty, FraudCusts = new List<GetFraudCustomerModel>() };

            try
            {
                if (!string.IsNullOrEmpty(encryptedData))
                {
                    var participant = fraudCustomerDBService.GetParticipant(participantID);

                    if (participant != null)
                    {
                        var decryptedData = CryptographicHelper.AESDecrypt(encryptedData, participant.SecretKey, participant.ParticipantName);

                        rs.FraudCusts = fraudCustomerDBService.GetFraudCustomers(decryptedData);
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - GetFraudCustomer", string.Empty, (participantID, encryptedData), ex, sentryClient);

                rs.ErrorCode = HttpStatus.BadRequest;
                rs.ErrorMsg = ex.Message;
            }

            return rs;
        }

        public string GetFraudCustomerDecryption(int? participantID)
        {
            try
            {
                return fraudCustomerDBService.GetFraudCustomerDecryption(participantID);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - GetFraudCustomer", string.Empty, (participantID), ex, sentryClient);
            }

            return string.Empty;
        }
    }
}