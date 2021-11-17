namespace AntiFraudCenterService.Features.FraudCustomer
{
    using AntiFraudCenterService.Features.ParticipantInformation.Model;
    using AntiFraudCenterService.Shares;
    using AntiFraudCenterService.Shares.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using static AntiFraudCenterService.Shares.Constants;

    public interface IParticipantInfoDBService
    {
        RegenerateKeyResultModel UpdateParticipantSecretKey(int? participantID, string newSecretKey);

        IEnumerable<FraudCasesMappingModel> GetFraudCases();

        IEnumerable<DocumentMappingModel> GetDocumentTypes();

        IEnumerable<CountryMappingModel> GetCountryCodes();

        InitParticipantResultModel InitParticipant(string participantName, string secretKey);

        bool IsPassWordValid(string password);
    }

    public class ParticipantInfoDBService : IParticipantInfoDBService
    {
        private readonly IParticipantInfoDataAccess participantInfoDataAccess;

        public ParticipantInfoDBService(IParticipantInfoDataAccess participantInfoDataAccess)
        {
            this.participantInfoDataAccess = participantInfoDataAccess;
        }

        public RegenerateKeyResultModel UpdateParticipantSecretKey(int? participantID, string newSecretKey)
        {
            if (!string.IsNullOrEmpty(newSecretKey) && participantID != null)
            {
                try
                {
                    return participantInfoDataAccess.UpdateParticipantSecretKey(participantID, newSecretKey);
                }
                catch (Exception ex)
                {
                    return new RegenerateKeyResultModel() { StatusCode = HttpStatus.ServerNotImplemented, ErrorMessage = ex.Message };
                }
            }

            return new RegenerateKeyResultModel() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
        }

        public IEnumerable<FraudCasesMappingModel> GetFraudCases()
        {
            return participantInfoDataAccess.GetFraudCases();
        }

        public IEnumerable<DocumentMappingModel> GetDocumentTypes()
        {
            return participantInfoDataAccess.GetDocumentTypes();
        }

        public IEnumerable<CountryMappingModel> GetCountryCodes()
        {
            var countryCodes = new List<CountryMappingModel>();

            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            foreach (var culture in cultures)
            {
                var region = new RegionInfo(culture.LCID);

                countryCodes.Add(new CountryMappingModel
                {
                    CountryName = culture.EnglishName,
                    CountryCode = region?.ThreeLetterISORegionName ?? string.Empty
                });
            }

            return countryCodes;
        }


        public InitParticipantResultModel InitParticipant(string participantName, string secretKey)
        {
            if (!string.IsNullOrEmpty(participantName) && !string.IsNullOrEmpty(secretKey))
            {
                return participantInfoDataAccess.InitParticipant(participantName, secretKey);
            }

            return new InitParticipantResultModel { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
        }

        public bool IsPassWordValid(string password)
        {
            if (!string.IsNullOrEmpty(password) && string.Equals(password, Constants.InternalPassword))
            {
                return true;
            }

            return false;
        }
    }
}