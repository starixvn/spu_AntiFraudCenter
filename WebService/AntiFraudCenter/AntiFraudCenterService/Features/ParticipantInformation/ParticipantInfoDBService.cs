namespace AntiFraudCenterService.Features.FraudCustomer
{
    using AntiFraudCenterService.Features.ParticipantInformation.Model;
    using AntiFraudCenterService.Shares.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using static AntiFraudCenterService.Shares.Constants;
    using static AntiFraudCenterService.Shares.CryptographicConfig;

    public interface IParticipantInfoDBService
    {
        RegenerateKeyResultModel UpdateParticipantSecretKey(int? participantID, string newSecretKey);

        IEnumerable<FraudCasesMappingModel> GetFraudCases();

        IEnumerable<DocumentMappingModel> GetDocumentTypes();

        IEnumerable<CountryMappingModel> GetCountryCodes();

        InitParticipantResultModel InitParticipant(string participantName, string secretKey, string ips);

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
                    return new RegenerateKeyResultModel() { StatusCode = HttpStatus.InternalServerError, ErrorMessage = ex.Message };
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
                    CountryName = region.EnglishName,
                    CountryCode = region?.ThreeLetterISORegionName ?? string.Empty
                });
            }

            if (countryCodes.Any())
            {
                return countryCodes.GroupBy(c => c.CountryCode).Select(i => i.FirstOrDefault()).ToList();
            }

            return new List<CountryMappingModel>();
        }

        public InitParticipantResultModel InitParticipant(string participantName, string secretKey, string ips)
        {
            if (!string.IsNullOrEmpty(participantName) && !string.IsNullOrEmpty(secretKey))
            {
                return participantInfoDataAccess.InitParticipant(participantName, secretKey, ips);
            }

            return new InitParticipantResultModel { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
        }

        public bool IsPassWordValid(string password)
        {
            if (!string.IsNullOrEmpty(password) && string.Equals(password, InternalPassword))
            {
                return true;
            }

            return false;
        }
    }
}