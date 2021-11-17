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
        GetFraudCustModelResult GetFraudCustomer(string key, string jsonData);

        FraudCustDBResult AddFraudCustomer(string key, string jsonData);

        FraudCustDBResult UpdateFraudCustomer(string key, string jsonData);

        FraudCustDBResult DeleteFraudCustomer(string key, string jsonData);
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

        public GetFraudCustModelResult GetFraudCustomer(string key, string jsonData)
        {
            var rs = new GetFraudCustModelResult() { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty, Data = string.Empty };

            try
            {
                if (string.IsNullOrEmpty(jsonData) || string.IsNullOrEmpty(key))
                {
                    return new GetFraudCustModelResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput, Data = string.Empty };
                }

                var participant = fraudCustomerDBService.CheckAuthentication(key);

                if (participant != null)
                {
                    var encryptedFields = fraudCustomerDBService.EncryptedFieldsToGetDB(jsonData);

                    var fraudCusts = fraudCustomerDBService.GetFraudCustomers(encryptedFields);

                    if (fraudCusts.Any())
                    {
                        var decryptedFields = fraudCustomerDBService.DecryptedFieldsFromDB(fraudCusts);

                        rs.Data = JsonConvert.SerializeObject(decryptedFields);
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - GetFraudCustomer", ex.Message, (jsonData, key), ex, sentryClient);

                rs.StatusCode = HttpStatus.BadRequest;
                rs.ErrorMessage = ex.Message;
            }

            return rs;
        }

        public FraudCustDBResult AddFraudCustomer(string key, string jsonData)
        {
            var rs = new FraudCustDBResult() { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty };

            try
            {
                if (string.IsNullOrEmpty(jsonData) || string.IsNullOrEmpty(key))
                {
                    return new FraudCustDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
                }

                var participant = fraudCustomerDBService.CheckAuthentication(key);

                if (participant != null)
                {
                    var encryptedFields = fraudCustomerDBService.EncryptedFieldsToInsertDB(jsonData);

                    rs = fraudCustomerDBService.AddFraudCustomers(encryptedFields, participant.ParticipantID);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - AddFraudCustomer", ex.Message, (jsonData, key), ex, sentryClient);

                rs.StatusCode = HttpStatus.BadRequest;
                rs.ErrorMessage = ex.Message;
            }

            return rs;
        }

        public FraudCustDBResult UpdateFraudCustomer(string key, string jsonData)
        {
            var rs = new FraudCustDBResult() { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty };

            try
            {
                if (string.IsNullOrEmpty(jsonData) || string.IsNullOrEmpty(key))
                {
                    return new FraudCustDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
                }

                var participant = fraudCustomerDBService.CheckAuthentication(key);

                if (participant != null)
                {
                    var encryptedFields = fraudCustomerDBService.EncryptedFieldsToInsertDB(jsonData);

                    rs = fraudCustomerDBService.UpdateFraudCustomers(encryptedFields, participant.ParticipantID);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - UpdateFraudCustomer", ex.Message, (jsonData, key), ex, sentryClient);

                rs.StatusCode = HttpStatus.BadRequest;
                rs.ErrorMessage = ex.Message;
            }

            return rs;
        }

        public FraudCustDBResult DeleteFraudCustomer(string key, string jsonData)
        {
            var rs = new FraudCustDBResult() { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty };

            try
            {
                if (string.IsNullOrEmpty(jsonData) || string.IsNullOrEmpty(key))
                {
                    return new FraudCustDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
                }

                var participant = fraudCustomerDBService.CheckAuthentication(key);

                if (participant != null)
                {
                    var encryptedFields = fraudCustomerDBService.EncryptedFieldsToGetDB(jsonData);

                    rs = fraudCustomerDBService.DeleteFraudCustomers(encryptedFields, participant.ParticipantID);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - DeleteFraudCustomer", ex.Message, (jsonData, key), ex, sentryClient);

                rs.StatusCode = HttpStatus.BadRequest;
                rs.ErrorMessage = ex.Message;
            }

            return rs;
        }
    }
}