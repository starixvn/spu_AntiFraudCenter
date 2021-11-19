namespace AntiFraudCenter
{
    using AntiFraudCenterService.Features.FraudCustomer;
    using AntiFraudCenterService.Shares;
    using AntiFraudCenterService.Shares.Model;
    using AntiFraudCenterService.Shares.ViewModel;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static AntiFraudCenterService.Shares.Constants;

    public interface IFraudCustomerService
    {
        GetFraudCustResultViewModel GetAccounts(string key, GetFraudCustParasModel data);

        FraudCustDBResult AddFraudCustomer(string key, IEnumerable<InsertFraudCustParasModel> data);

        FraudCustDBResult UpdateFraudCustomer(string key, IEnumerable<InsertFraudCustParasModel> data);

        FraudCustDBResult DeleteFraudCustomer(string key, IEnumerable<DeleteFraudCustParasModel> accountIDs);
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

        public GetFraudCustResultViewModel GetAccounts(string key, GetFraudCustParasModel data)
        {
            var rs = new GetFraudCustResultViewModel() { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty, Data = new List<GetFraudCustResultModel>() };

            try
            {
                if (data == null || string.IsNullOrEmpty(key))
                {
                    return new GetFraudCustResultViewModel() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput, Data = new List<GetFraudCustResultModel>() };
                }

                var participant = fraudCustomerDBService.CheckAuthentication(key);

                if (participant != null)
                {
                    var encryptedFields = fraudCustomerDBService.EncryptedFieldsToGetDB(data);

                    var fraudCusts = fraudCustomerDBService.GetFraudCustomers(encryptedFields);

                    if (fraudCusts.Any())
                    {
                        rs.Data = fraudCustomerDBService.DecryptedFieldsFromDB(fraudCusts);
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - GetFraudCustomer", ex.Message, (data, key), ex, sentryClient);

                rs.StatusCode = HttpStatus.BadRequest;
                rs.ErrorMessage = ex.Message;
            }

            return rs;
        }

        public FraudCustDBResult AddFraudCustomer(string key, IEnumerable<InsertFraudCustParasModel> data)
        {
            var rs = new FraudCustDBResult() { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty };

            try
            {
                if (data == null || !data.Any() || string.IsNullOrEmpty(key))
                {
                    return new FraudCustDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
                }

                var participant = fraudCustomerDBService.CheckAuthentication(key);

                if (participant != null)
                {
                    var encryptedData = fraudCustomerDBService.EncryptedFieldsToInsertDB(data);

                    rs = fraudCustomerDBService.AddFraudCustomers(encryptedData, participant.ParticipantID);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - AddFraudCustomer", ex.Message, (data, key), ex, sentryClient);

                rs.StatusCode = HttpStatus.BadRequest;
                rs.ErrorMessage = ex.Message;
            }

            return rs;
        }

        public FraudCustDBResult UpdateFraudCustomer(string key, IEnumerable<InsertFraudCustParasModel> data)
        {
            var rs = new FraudCustDBResult() { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty };

            try
            {
                if (data == null || !data.Any() || string.IsNullOrEmpty(key))
                {
                    return new FraudCustDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
                }

                var participant = fraudCustomerDBService.CheckAuthentication(key);

                if (participant != null)
                {
                    var encryptedData = fraudCustomerDBService.EncryptedFieldsToInsertDB(data);

                    rs = fraudCustomerDBService.UpdateFraudCustomers(encryptedData, participant.ParticipantID);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - UpdateFraudCustomer", ex.Message, (data, key), ex, sentryClient);

                rs.StatusCode = HttpStatus.BadRequest;
                rs.ErrorMessage = ex.Message;
            }

            return rs;
        }

        public FraudCustDBResult DeleteFraudCustomer(string key, IEnumerable<DeleteFraudCustParasModel> accountIDs)
        {
            var rs = new FraudCustDBResult() { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty };

            try
            {
                if (accountIDs == null || !accountIDs.Any() || string.IsNullOrEmpty(key))
                {
                    return new FraudCustDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
                }

                var participant = fraudCustomerDBService.CheckAuthentication(key);

                if (participant != null)
                {
                    rs = fraudCustomerDBService.DeleteFraudCustomers(accountIDs, participant.ParticipantID);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - DeleteFraudCustomer", ex.Message, (accountIDs, key), ex, sentryClient);

                rs.StatusCode = HttpStatus.BadRequest;
                rs.ErrorMessage = ex.Message;
            }

            return rs;
        }
    }
}