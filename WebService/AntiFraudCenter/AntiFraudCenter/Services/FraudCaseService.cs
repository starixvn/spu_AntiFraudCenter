namespace AntiFraudCenter
{
    using AntiFraudCenterService.Features.FraudCustomer;
    using AntiFraudCenterService.Shares;
    using AntiFraudCenterService.Shares.Model;
    using AntiFraudCenterService.Shares.ViewModel;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Linq;
    using static AntiFraudCenterService.Shares.Constants;

    public interface IFraudCustomerService
    {
        GetFraudCustModelResult GetFraudCustomer(int? participantID, string encryptedData);

        string GetFraudCustomerDecryption(int? participantID);

        FraudCustDBResult AddFraudCustomer(int? participantID, string encryptedData);

        FraudCustDBResult UpdateFraudCustomer(int? participantID, string encryptedData);

        FraudCustDBResult DeleteFraudCustomer(int? participantID, string encryptedData);
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

        public GetFraudCustModelResult GetFraudCustomer(int? participantID, string encryptedData)
        {
            var rs = new GetFraudCustModelResult() { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty, EncryptedData = string.Empty };

            try
            {
                if (string.IsNullOrEmpty(encryptedData) || participantID == null)
                {
                    return new GetFraudCustModelResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput, EncryptedData = string.Empty };
                }

                var participant = fraudCustomerDBService.GetParticipant(participantID);

                if (participant != null)
                {
                    var decryptedData = CryptographicHelper.AESDecrypt(encryptedData, participant.SecretKey, participant.ParticipantName);

                    var fraudCusts = fraudCustomerDBService.GetFraudCustomers(decryptedData);

                    if (fraudCusts.Any())
                    {
                        rs.EncryptedData = CryptographicHelper.AESEncrypt(JsonConvert.SerializeObject(fraudCusts), participant.SecretKey, participant.ParticipantName);
                    }
                    else
                    {
                        rs.StatusCode = HttpStatus.NotFound;
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - GetFraudCustomer", string.Empty, (participantID, encryptedData), ex, sentryClient);

                rs.StatusCode = HttpStatus.BadRequest;
                rs.ErrorMessage = ex.Message;
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

        public FraudCustDBResult AddFraudCustomer(int? participantID, string encryptedData)
        {
            var rs = new FraudCustDBResult() { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty };

            try
            {
                if (string.IsNullOrEmpty(encryptedData) || participantID == null)
                {
                    return new FraudCustDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
                }

                var participant = fraudCustomerDBService.GetParticipant(participantID);

                if (participant != null)
                {
                    var decryptedData = CryptographicHelper.AESDecrypt(encryptedData, participant.SecretKey, participant.ParticipantName);

                    rs = fraudCustomerDBService.AddFraudCustomers(decryptedData, participantID);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - AddFraudCustomer", string.Empty, (participantID, encryptedData), ex, sentryClient);

                rs.StatusCode = HttpStatus.BadRequest;
                rs.ErrorMessage = ex.Message;
            }

            return rs;
        }

        public FraudCustDBResult UpdateFraudCustomer(int? participantID, string encryptedData)
        {
            var rs = new FraudCustDBResult() { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty };

            try
            {
                if (string.IsNullOrEmpty(encryptedData) || participantID == null)
                {
                    return new FraudCustDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
                }

                var participant = fraudCustomerDBService.GetParticipant(participantID);

                if (participant != null)
                {
                    var decryptedData = CryptographicHelper.AESDecrypt(encryptedData, participant.SecretKey, participant.ParticipantName);

                    rs = fraudCustomerDBService.UpdateFraudCustomers(decryptedData, participantID);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - UpdateFraudCustomer", string.Empty, (participantID, encryptedData), ex, sentryClient);

                rs.StatusCode = HttpStatus.BadRequest;
                rs.ErrorMessage = ex.Message;
            }

            return rs;
        }

        public FraudCustDBResult DeleteFraudCustomer(int? participantID, string encryptedData)
        {
            var rs = new FraudCustDBResult() { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty };

            try
            {
                if (string.IsNullOrEmpty(encryptedData) || participantID == null)
                {
                    return new FraudCustDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
                }

                var participant = fraudCustomerDBService.GetParticipant(participantID);

                if (participant != null)
                {
                    var decryptedData = CryptographicHelper.AESDecrypt(encryptedData, participant.SecretKey, participant.ParticipantName);

                    rs = fraudCustomerDBService.DeleteFraudCustomers(decryptedData, participantID);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - DeleteFraudCustomer", string.Empty, (participantID, encryptedData), ex, sentryClient);

                rs.StatusCode = HttpStatus.BadRequest;
                rs.ErrorMessage = ex.Message;
            }

            return rs;
        }
    }
}