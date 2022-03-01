namespace AntiFraudCenter
{
    using AntiFraudCenterService.Features.Accounts;
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

        GetAccountByDateResultViewModel GetAccountsByDate(string key, GetAccountByDateParasModel paras);

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

                bool isInternalRequest = Equals(appSettings.AFCInternalKey, key);

                var participant = accountDBService.CheckAuthentication(key, isInternalRequest);

                if (participant != null)
                {
                    var encryptedFields = accountDBService.EncryptAccount(data);

                    var accounts = accountDBService.GetAccounts(encryptedFields, isInternalRequest);

                    if (accounts.Any())
                    {
                        rs.Data = accountDBService.DecryptAccount(accounts);
                    }
                }
                else
                {
                    rs.StatusCode = HttpStatus.BadRequest;
                    rs.ErrorMessage = InvalidKey;
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

        public GetAccountByDateResultViewModel GetAccountsByDate(string key, GetAccountByDateParasModel paras)
        {
            var result = new GetAccountByDateResultViewModel() { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty, Data = new GetAccountByDateResultModel() };

            try
            {
                if (paras == null || string.IsNullOrEmpty(key))
                {
                    return new GetAccountByDateResultViewModel() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput, Data = new GetAccountByDateResultModel() };
                }

                bool isInternalRequest = Equals(appSettings.AFCInternalKey, key);

                var participant = accountDBService.CheckAuthentication(key, isInternalRequest);

                if (participant != null)
                {
                    var queryData = accountDBService.GetAccountsByDate(paras, participant.ParticipantID, isInternalRequest);

                    if (queryData != null && queryData.Data != null && queryData.Data.Any())
                    {
                        result.Data.Data = accountDBService.DecryptAccount(queryData.Data);
                        result.Data.IsContinue = queryData.IsContinue;
                        result.Data.DateFrom = queryData.DateFrom;
                        result.Data.DateTo = queryData.DateTo;
                    }
                }
                else
                {
                    result.StatusCode = HttpStatus.BadRequest;
                    result.ErrorMessage = InvalidKey;
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - GetAccountsByDate", ex.Message, (paras, key), ex, sentryClient);

                result.StatusCode = HttpStatus.BadRequest;

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public AccountDBResult InsertAccounts(string key, IEnumerable<InsertAccountParasModel> data)
        {
            var rs = new AccountDBResult() { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty };
            ParticipantModel participant = null;

            try
            {
                if (data == null || !data.Any() || string.IsNullOrEmpty(key))
                {
                    return new AccountDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
                }

                bool isInternalRequest = Equals(appSettings.AFCInternalKey, key);

                participant = accountDBService.CheckAuthentication(key, isInternalRequest);

                if (participant == null)
                {
                    return new AccountDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidKey };
                }

                LogMonitorRequest(isInternalRequest, RequestAction.Insert, participant.ParticipantName, data?.Count());

                var dataValid = CheckValidation(participant, RequestAction.Insert, isInternalRequest, data.Select(c => new AccountValidationParaModel
                {
                    AccountID = c.AccountID,
                    CountryCode = c.CountryCode,
                    Phone = c.Phone
                }));

                if (dataValid == null || dataValid.StatusCode != HttpStatus.OK)
                {
                    return dataValid;
                }

                var encryptedData = accountDBService.EncryptAccount(data);

                rs = accountDBService.InsertAccounts(encryptedData, participant.ParticipantID, isInternalRequest, appSettings.InternalBatchSize);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - InsertAccounts", ex.Message, (data, key), ex, sentryClient);
                rs.StatusCode = HttpStatus.BadRequest;
                rs.ErrorMessage = ex.Message;

                accountDBService.LogErrorToDB(participant?.ParticipantID, ex.Message, JsonConvert.SerializeObject(data), Equals(appSettings.AFCInternalKey, key));
            }

            return rs;
        }

        public AccountDBResult UpdateAccounts(string key, IEnumerable<InsertAccountParasModel> data)
        {
            var rs = new AccountDBResult() { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty };
            ParticipantModel participant = null;

            try
            {
                if (data == null || !data.Any() || string.IsNullOrEmpty(key))
                {
                    return new AccountDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
                }

                bool isInternalRequest = Equals(appSettings.AFCInternalKey, key);

                participant = accountDBService.CheckAuthentication(key, isInternalRequest);

                if (participant == null)
                {
                    return new AccountDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidKey };
                }

                LogMonitorRequest(isInternalRequest, RequestAction.Update, participant.ParticipantName, data?.Count());

                var dataValid = CheckValidation(participant, RequestAction.Update, isInternalRequest, data.Select(c => new AccountValidationParaModel
                {
                    AccountID = c.AccountID,
                    CountryCode = c.CountryCode,
                    Phone = c.Phone
                }));

                if (dataValid == null || dataValid.StatusCode != HttpStatus.OK)
                {
                    return dataValid;
                }

                var encryptedData = accountDBService.EncryptAccount(data);

                rs = accountDBService.UpdateAccounts(encryptedData, participant.ParticipantID, isInternalRequest, appSettings.InternalBatchSize);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - UpdateAccounts", ex.Message, (data, key), ex, sentryClient);
                rs.StatusCode = HttpStatus.BadRequest;
                rs.ErrorMessage = ex.Message;

                accountDBService.LogErrorToDB(participant?.ParticipantID, ex.Message, JsonConvert.SerializeObject(data), Equals(appSettings.AFCInternalKey, key));
            }

            return rs;
        }

        public AccountDBResult DeleteAccounts(string key, IEnumerable<DeleteAccountParasModel> accountIDs)
        {
            var rs = new AccountDBResult() { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty };
            ParticipantModel participant = null;

            try
            {
                if (accountIDs == null || !accountIDs.Any() || string.IsNullOrEmpty(key))
                {
                    return new AccountDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
                }

                bool isInternalRequest = Equals(appSettings.AFCInternalKey, key);

                participant = accountDBService.CheckAuthentication(key, isInternalRequest);

                if (participant == null)
                {
                    return new AccountDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidKey };
                }

                LogMonitorRequest(isInternalRequest, RequestAction.Update, participant.ParticipantName, accountIDs?.Count());

                var dataValid = CheckValidation(participant, RequestAction.Delete, isInternalRequest, accountIDs.Select(c => new AccountValidationParaModel
                {
                    AccountID = c.AccountID
                }));

                if (dataValid == null || dataValid.StatusCode != HttpStatus.OK)
                {
                    return dataValid;
                }

                rs = accountDBService.DeleteAccounts(accountIDs, participant.ParticipantID, isInternalRequest, appSettings.InternalBatchSize);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - DeleteAccounts", ex.Message, (accountIDs, key), ex, sentryClient);
                rs.StatusCode = HttpStatus.BadRequest;
                rs.ErrorMessage = ex.Message;

                accountDBService.LogErrorToDB(participant?.ParticipantID, ex.Message, JsonConvert.SerializeObject(accountIDs), Equals(appSettings.AFCInternalKey, key));
            }

            return rs;
        }

        public AccountDBResult CheckValidation(ParticipantModel participant, RequestAction action, bool isInternalRequest, IEnumerable<AccountValidationParaModel> data)
        {
            if (data == null || !data.Any())
            {
                return new AccountDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
            }

            var result = accountDBService.CheckDataValidation(data, participant.ParticipantID, action, isInternalRequest);

            return result;
        }

        #region Private Methods

        private void LogMonitorRequest(bool isInternalRequest, RequestAction requestAction, string participantName, int? recordNumber)
        {
            if (isInternalRequest)
            {
                return;
            }

            var message = $"Monitor - {Enum.GetName(typeof(RequestAction), requestAction)} - {participantName} - Counter:{recordNumber} - DateTime:{DateTime.UtcNow}";

            sentryClient.CaptureMessage(message, Sentry.Protocol.SentryLevel.Info);
        }

        #endregion Private Methods
    }
}