namespace AntiFraudCenter
{
    using AntiFraudCenterService.Features.FraudCustomer;
    using AntiFraudCenterService.Features.ParticipantInformation.Model;
    using AntiFraudCenterService.Shares;
    using AntiFraudCenterService.Shares.ViewModel;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using static AntiFraudCenterService.Shares.Constants;

    public interface IParticipantService
    {
        RegenerateKeyResultModel UpdateParticipantSecretKey(string key);

        ConventionViewModel GetConventionInfo();

        InitParticipantResultModel InitParticipant(string participantName, string password, string ips);
    }

    public class ParticipantService : IParticipantService
    {
        private readonly ISentryClient sentryClient;
        private readonly IParticipantInfoDBService participantInfoDBService;
        private readonly IAccountDBService fraudCustomerDBService;
        private readonly IAppSettings appSettings;

        public ParticipantService(
            ISentryClient sentryClient,
            IParticipantInfoDBService participantInfoDBService,
            IAccountDBService fraudCustomerDBService,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.participantInfoDBService = participantInfoDBService;
            this.fraudCustomerDBService = fraudCustomerDBService;
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

                var participant = fraudCustomerDBService.CheckAuthentication(key);

                if (participant != null)
                {
                    var newKey = CryptographicHelper.GetRandomHexNumber(SecretKey_NumberOfByte);

                    rs = participantInfoDBService.UpdateParticipantSecretKey(participant.ParticipantID, newKey);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("AFC - UpdateParticipantSecretKey", string.Empty, (key), ex, sentryClient);

                rs.ErrorMessage = ex.Message;
            }

            return rs;
        }

        public ConventionViewModel GetConventionInfo()
        {
            var rs = new ConventionViewModel
            {
                DocumentMapping = new List<DocumentMappingModel>(),
                FraudCasesMapping = new List<FraudCasesMappingModel>(),
                CountryMapping = new List<CountryMappingModel>(),
                StatusCode = HttpStatus.OK,
                ErrorMessage = string.Empty
            };

            try
            {
                rs.DocumentMapping = participantInfoDBService.GetDocumentTypes();
                rs.FraudCasesMapping = participantInfoDBService.GetFraudCases();
                rs.CountryMapping = participantInfoDBService.GetCountryCodes();
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
                    bool isValid = participantInfoDBService.IsPassWordValid(password);

                    if (isValid)
                    {
                        var generateKey = CryptographicHelper.GetRandomHexNumber(SecretKey_NumberOfByte);

                        rs = participantInfoDBService.InitParticipant(participantName, generateKey, ips);
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
    }
}