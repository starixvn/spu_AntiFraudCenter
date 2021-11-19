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

    public interface IAccountService
    {
        GetAccountResultViewModel GetAccounts(string key, GetAccountParasModel data);

        AccountDBResult InsertAccounts(string key, IEnumerable<InsertAccountParasModel> data);

        AccountDBResult UpdateAccounts(string key, IEnumerable<InsertAccountParasModel> data);

        AccountDBResult DeleteAccounts(string key, IEnumerable<DeleteAccountParasModel> accountIDs);
    }

    public class AccountService : IAccountService
    {
        private readonly ISentryClient sentryClient;
        private readonly IAccountDBService accountDBService;
        private readonly IAppSettings appSettings;

        public AccountService(
            ISentryClient sentryClient,
            IAccountDBService accountService,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.accountDBService = accountService;
            this.appSettings = appSettings;
        }

        public GetAccountResultViewModel GetAccounts(string key, GetAccountParasModel data)
        {
            var rs = new GetAccountResultViewModel() { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty, Data = new List<GetAccountResultModel>() };

            try
            {
                if (data == null || string.IsNullOrEmpty(key))
                {
                    return new GetAccountResultViewModel() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput, Data = new List<GetAccountResultModel>() };
                }

                var participant = accountDBService.CheckAuthentication(key);

                if (participant != null)
                {
                    var encryptedFields = accountDBService.EncryptedFieldsToGetDB(data);

                    var fraudCusts = accountDBService.GetAccounts(encryptedFields);

                    if (fraudCusts.Any())
                    {
                        rs.Data = accountDBService.DecryptedFieldsFromDB(fraudCusts);
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - GetAccounts", ex.Message, (data, key), ex, sentryClient);

                rs.StatusCode = HttpStatus.BadRequest;
                rs.ErrorMessage = ex.Message;
            }

            return rs;
        }

        public AccountDBResult InsertAccounts(string key, IEnumerable<InsertAccountParasModel> data)
        {
            var rs = new AccountDBResult() { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty };

            try
            {
                if (data == null || !data.Any() || string.IsNullOrEmpty(key))
                {
                    return new AccountDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
                }

                var participant = accountDBService.CheckAuthentication(key);

                if (participant != null)
                {
                    var encryptedData = accountDBService.EncryptedFieldsToInsertDB(data);

                    rs = accountDBService.InsertAccounts(encryptedData, participant.ParticipantID);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - InsertAccounts", ex.Message, (data, key), ex, sentryClient);

                rs.StatusCode = HttpStatus.BadRequest;
                rs.ErrorMessage = ex.Message;
            }

            return rs;
        }

        public AccountDBResult UpdateAccounts(string key, IEnumerable<InsertAccountParasModel> data)
        {
            var rs = new AccountDBResult() { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty };

            try
            {
                if (data == null || !data.Any() || string.IsNullOrEmpty(key))
                {
                    return new AccountDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
                }

                var participant = accountDBService.CheckAuthentication(key);

                if (participant != null)
                {
                    var encryptedData = accountDBService.EncryptedFieldsToInsertDB(data);

                    rs = accountDBService.UpdateAccounts(encryptedData, participant.ParticipantID);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - UpdateAccounts", ex.Message, (data, key), ex, sentryClient);

                rs.StatusCode = HttpStatus.BadRequest;
                rs.ErrorMessage = ex.Message;
            }

            return rs;
        }

        public AccountDBResult DeleteAccounts(string key, IEnumerable<DeleteAccountParasModel> accountIDs)
        {
            var rs = new AccountDBResult() { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty };

            try
            {
                if (accountIDs == null || !accountIDs.Any() || string.IsNullOrEmpty(key))
                {
                    return new AccountDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
                }

                var participant = accountDBService.CheckAuthentication(key);

                if (participant != null)
                {
                    rs = accountDBService.DeleteAccounts(accountIDs, participant.ParticipantID);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - DeleteAccounts", ex.Message, (accountIDs, key), ex, sentryClient);

                rs.StatusCode = HttpStatus.BadRequest;
                rs.ErrorMessage = ex.Message;
            }

            return rs;
        }
    }
}