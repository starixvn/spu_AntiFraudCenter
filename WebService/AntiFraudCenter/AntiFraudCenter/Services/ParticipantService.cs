namespace AntiFraudCenter
{
    using AntiFraudCenterService.Features.FraudCustomer;
    using AntiFraudCenterService.Features.Participants;
    using AntiFraudCenterService.Features.Participants.Model;
    using AntiFraudCenterService.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using static AntiFraudCenterService.Shares.Constants;
    using static AntiFraudCenterService.Shares.CryptographicConfig;

    public interface IParticipantService
    {
        RegenerateKeyResultModel UpdateParticipantSecretKey(string key);

        ConventionResultModel GetConventionInfo();

        InitParticipantResultModel InitParticipant(string participantName, string password, string ips);

        GetParticipantNameResultModel GetParticipantName();
    }

    public class ParticipantService : IParticipantService
    {
        private readonly ISentryClient sentryClient;
        private readonly IParticipantInfoDBService participantDBService;
        private readonly IAccountDBService accountDBService;
        private readonly IAppSettings appSettings;

        public ParticipantService(
            ISentryClient sentryClient,
            IParticipantInfoDBService participantInfoDBService,
            IAccountDBService accountDBService,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.participantDBService = participantInfoDBService;
            this.accountDBService = accountDBService;
            this.appSettings = appSettings;
        }

        public RegenerateKeyResultModel UpdateParticipantSecretKey(string key)
        {
            var rs = new RegenerateKeyResultModel() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };

            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    return rs;
                }

                bool isInternalRequest = Equals(appSettings.AFCInternalKey, key);

                var participant = accountDBService.CheckAuthentication(key, isInternalRequest);

                if (participant != null)
                {
                    var newKey = CryptographicHelper.GetRandomHexNumber(SecretKey_NumberOfByte);

                    rs = participantDBService.UpdateParticipantSecretKey(participant.ParticipantID, newKey, isInternalRequest);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - UpdateParticipantSecretKey", string.Empty, (key), ex, sentryClient);

                rs.ErrorMessage = ex.Message;
            }

            return rs;
        }

        public ConventionResultModel GetConventionInfo()
        {
            var rs = new ConventionResultModel
            {
                DocumentMapping = new List<DocumentMappingModel>(),
                FraudCasesMapping = new List<FraudCasesMappingModel>(),
                CountryMapping = new List<CountryMappingModel>(),
                StatusCode = HttpStatus.OK,
                ErrorMessage = string.Empty
            };

            try
            {
                rs.DocumentMapping = participantDBService.GetDocumentTypes();
                rs.FraudCasesMapping = participantDBService.GetFraudCases();
                rs.CountryMapping = participantDBService.GetCountryCodes();
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - GetConventionInfo", string.Empty, string.Empty, ex, sentryClient);
                rs.ErrorMessage = ex.Message;
                rs.StatusCode = HttpStatus.InternalServerError;
            }

            return rs;
        }

        public InitParticipantResultModel InitParticipant(string participantName, string password, string ips)
        {
            var rs = new InitParticipantResultModel { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };

            try
            {
                if (!string.IsNullOrEmpty(participantName) && !string.IsNullOrEmpty(password))
                {
                    bool isValid = participantDBService.IsPassWordValid(password);

                    if (isValid)
                    {
                        var generateKey = CryptographicHelper.GetRandomHexNumber(SecretKey_NumberOfByte);

                        rs = participantDBService.InitParticipant(participantName, generateKey, ips);
                    }
                    else
                    {
                        rs.StatusCode = HttpStatus.BadRequest;
                        rs.ErrorMessage = InvalidKey;
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - InitParticipant", string.Empty, participantName, ex, sentryClient);
                rs.ErrorMessage = ex.Message;
                rs.StatusCode = HttpStatus.InternalServerError;
            }

            return rs;
        }

        public GetParticipantNameResultModel GetParticipantName()
        {
            var rs = new GetParticipantNameResultModel
            {
                StatusCode = HttpStatus.OK,
                ErrorMessage = string.Empty
            };

            try
            {
                rs.Data = participantDBService.GetParticipantName(false);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - GetParticipantName", string.Empty, string.Empty, ex, sentryClient);
                rs.ErrorMessage = ex.Message;
                rs.StatusCode = HttpStatus.InternalServerError;
            }

            return rs;
        }
    }
}