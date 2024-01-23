namespace AntiFraudCenter
{
    using AntiFraudCenterService.Features.AccountMaterials;
    using AntiFraudCenterService.Features.Accounts;
    using AntiFraudCenterService.Features.Authorization;
    using AntiFraudCenterService.Shares;
    using AntiFraudCenterService.Shares.Model;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static AntiFraudCenterService.Shares.Constants;

    public interface IAccountMaterialService
    {
        AccountDBResult InsertIPs(string key, IEnumerable<InsertIPParasModel> data);

        AccountDBResult InsertDevices(string key, IEnumerable<InsertDeviceParasModel> data);

        AccountDBResult DeleteIPs(string key, IEnumerable<DeleteIPParasModel> data);

        AccountDBResult DeleteDevices(string key, IEnumerable<DeleteDeviceParasModel> data);
    }

    public class AccountMaterialService : IAccountMaterialService
    {
        private readonly ISentryClient sentryClient;
        private readonly IAccountDBService accountDBService;
        private readonly IAppSettings appSettings;
        private readonly IAuthorizationDBService authorizationDBService;
        private readonly IAccountMaterialDBService accountMaterialDBService;

        public AccountMaterialService(
            ISentryClient sentryClient,
            IAccountDBService accountService,
            IAppSettings appSettings,
            IAuthorizationDBService authorizationDBService,
            IAccountMaterialDBService accountMaterialDBService)
        {
            this.sentryClient = sentryClient;
            this.accountDBService = accountService;
            this.appSettings = appSettings;
            this.authorizationDBService = authorizationDBService;
            this.accountMaterialDBService = accountMaterialDBService;
        }

        public AccountDBResult InsertIPs(string key, IEnumerable<InsertIPParasModel> data)
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

                Utilities.LogMonitorRequest(isInternalRequest, RequestAction.InsertIP, participant.ParticipantName, data?.Count(), sentryClient);

                var dataValid = authorizationDBService.CheckAccountPermisson(data.Select(c => new AccountsModel { AccountID = c.AccountID }), participant.ParticipantID, isInternalRequest);

                if (dataValid == null || dataValid.StatusCode != HttpStatus.OK)
                {
                    return dataValid;
                }

                rs = accountMaterialDBService.InsertIPs(data, participant.ParticipantID, isInternalRequest, appSettings.InternalBatchSize);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - InsertIPs", ex.Message, (data, key), ex, sentryClient);
                rs.StatusCode = HttpStatus.BadRequest;
                rs.ErrorMessage = ex.Message;

                accountDBService.LogErrorToDB(participant?.ParticipantID, ex.Message, JsonConvert.SerializeObject(data), Equals(appSettings.AFCInternalKey, key));
            }

            return rs;
        }

        public AccountDBResult InsertDevices(string key, IEnumerable<InsertDeviceParasModel> data)
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

                Utilities.LogMonitorRequest(isInternalRequest, RequestAction.InsertDevice, participant.ParticipantName, data?.Count(), sentryClient);

                var dataValid = authorizationDBService.CheckAccountPermisson(data.Select(c => new AccountsModel { AccountID = c.AccountID }), participant.ParticipantID, isInternalRequest);

                if (dataValid == null || dataValid.StatusCode != HttpStatus.OK)
                {
                    return dataValid;
                }

                rs = accountMaterialDBService.InsertDevices(data, participant.ParticipantID, isInternalRequest, appSettings.InternalBatchSize);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - InsertDevices", ex.Message, (data, key), ex, sentryClient);
                rs.StatusCode = HttpStatus.BadRequest;
                rs.ErrorMessage = ex.Message;

                accountDBService.LogErrorToDB(participant?.ParticipantID, ex.Message, JsonConvert.SerializeObject(data), Equals(appSettings.AFCInternalKey, key));
            }

            return rs;
        }

        public AccountDBResult DeleteIPs(string key, IEnumerable<DeleteIPParasModel> data)
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

                Utilities.LogMonitorRequest(isInternalRequest, RequestAction.DeleteIP, participant.ParticipantName, data?.Count(), sentryClient);

                var dataValid = authorizationDBService.CheckAccountPermisson(data.Select(c => new AccountsModel { AccountID = c.AccountID }), participant.ParticipantID, isInternalRequest);

                if (dataValid == null || dataValid.StatusCode != HttpStatus.OK)
                {
                    return dataValid;
                }

                rs = accountMaterialDBService.DeleteIPs(data, participant.ParticipantID, isInternalRequest, appSettings.InternalBatchSize);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - DeleteIPs", ex.Message, (data, key), ex, sentryClient);
                rs.StatusCode = HttpStatus.BadRequest;
                rs.ErrorMessage = ex.Message;

                accountDBService.LogErrorToDB(participant?.ParticipantID, ex.Message, JsonConvert.SerializeObject(data), Equals(appSettings.AFCInternalKey, key));
            }

            return rs;
        }

        public AccountDBResult DeleteDevices(string key, IEnumerable<DeleteDeviceParasModel> data)
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

                Utilities.LogMonitorRequest(isInternalRequest, RequestAction.DeleteDevice, participant.ParticipantName, data?.Count(), sentryClient);

                var dataValid = authorizationDBService.CheckAccountPermisson(data.Select(c => new AccountsModel { AccountID = c.AccountID }), participant.ParticipantID, isInternalRequest);

                if (dataValid == null || dataValid.StatusCode != HttpStatus.OK)
                {
                    return dataValid;
                }

                rs = accountMaterialDBService.DeleteDevices(data, participant.ParticipantID, isInternalRequest, appSettings.InternalBatchSize);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - DeleteDevices", ex.Message, (data, key), ex, sentryClient);
                rs.StatusCode = HttpStatus.BadRequest;
                rs.ErrorMessage = ex.Message;

                accountDBService.LogErrorToDB(participant?.ParticipantID, ex.Message, JsonConvert.SerializeObject(data), Equals(appSettings.AFCInternalKey, key));
            }

            return rs;
        }
    }
}