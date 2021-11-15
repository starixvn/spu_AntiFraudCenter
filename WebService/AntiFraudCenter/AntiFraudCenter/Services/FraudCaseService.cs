namespace AntiFraudCenter
{
    using AntiFraudCenterService.Features.FraudCustomer;
    using AntiFraudCenterService.Shares;
    using AntiFraudCenterService.Shares.Model;
    using AntiFraudCenterService.Shares.ViewModel;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using static AntiFraudCenterService.Shares.Constants;

    public interface IFraudCustomerService
    {
        FraudCustResult GetFraudCustomer(int? participantID, string encryptedData);

        string GetFraudCustomerDecryption(int? participantID);

        DBResult AddFraudCustomer(int? participantID, string encryptedData);
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

        public DBResult AddFraudCustomer(int? participantID, string encryptedData)
        {
            var rs = new DBResult() { ErrorCode = HttpStatus.OK, ErrorMsg = string.Empty };

            try
            {
                if (!string.IsNullOrEmpty(encryptedData))
                {
                    var participant = fraudCustomerDBService.GetParticipant(participantID);

                    if (participant != null)
                    {
                        var decryptedData = CryptographicHelper.AESDecrypt(encryptedData, participant.SecretKey, participant.ParticipantName);

                        rs = fraudCustomerDBService.AddFraudCustomers(decryptedData, participantID);
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - AddFraudCustomer", string.Empty, (participantID, encryptedData), ex, sentryClient);

                rs.ErrorCode = HttpStatus.BadRequest;
                rs.ErrorMsg = ex.Message;
            }

            return rs;
        }

        public DBResult UpdateFraudCustomer(int? participantID, string encryptedData)
        {
            var rs = new DBResult() { ErrorCode = HttpStatus.OK, ErrorMsg = string.Empty };

            try
            {
                if (!string.IsNullOrEmpty(encryptedData))
                {
                    var participant = fraudCustomerDBService.GetParticipant(participantID);

                    if (participant != null)
                    {
                        var decryptedData = CryptographicHelper.AESDecrypt(encryptedData, participant.SecretKey, participant.ParticipantName);

                        rs = fraudCustomerDBService.UpdateFraudCustomers(decryptedData, participantID);
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - UpdateFraudCustomer", string.Empty, (participantID, encryptedData), ex, sentryClient);

                rs.ErrorCode = HttpStatus.BadRequest;
                rs.ErrorMsg = ex.Message;
            }

            return rs;
        }

        public DBResult DeleteFraudCustomer(int? participantID, string encryptedData)
        {
            var rs = new DBResult() { ErrorCode = HttpStatus.OK, ErrorMsg = string.Empty };

            try
            {
                if (!string.IsNullOrEmpty(encryptedData))
                {
                    var participant = fraudCustomerDBService.GetParticipant(participantID);

                    if (participant != null)
                    {
                        var decryptedData = CryptographicHelper.AESDecrypt(encryptedData, participant.SecretKey, participant.ParticipantName);

                        rs = fraudCustomerDBService.DeleteFraudCustomers(decryptedData, participantID);
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - DeleteFraudCustomer", string.Empty, (participantID, encryptedData), ex, sentryClient);

                rs.ErrorCode = HttpStatus.BadRequest;
                rs.ErrorMsg = ex.Message;
            }

            return rs;
        }
    }
}