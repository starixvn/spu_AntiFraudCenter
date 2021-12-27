namespace AntiFraudCenterService.Features.Participants
{
    using AntiFraudCenterService.Features.Participants.Model;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using static AntiFraudCenterService.Shares.Constants;
    using static AntiFraudCenterService.Shares.CryptographicConfig;

    public interface IParticipantInfoDBService
    {
        RegenerateKeyResultModel UpdateParticipantSecretKey(int? participantID, string newSecretKey, bool isInternalRequest);

        IEnumerable<FraudCasesMappingModel> GetFraudCases();

        IEnumerable<DocumentMappingModel> GetDocumentTypes();

        IEnumerable<CountryMappingModel> GetCountryCodes();

        InitParticipantResultModel InitParticipant(string participantName, string secretKey, string ips);

        bool IsPassWordValid(string password);

        IEnumerable<string> GetParticipantName(bool isInternalRequest);
    }

    public class ParticipantInfoDBService : IParticipantInfoDBService
    {
        private readonly IParticipantInfoDataAccess dataAccess;

        public ParticipantInfoDBService(IParticipantInfoDataAccess participantInfoDataAccess)
        {
            this.dataAccess = participantInfoDataAccess;
        }

        public RegenerateKeyResultModel UpdateParticipantSecretKey(int? participantID, string newSecretKey, bool isInternalRequest)
        {
            if (!string.IsNullOrEmpty(newSecretKey) && participantID != null)
            {
                try
                {
                    return dataAccess.UpdateParticipantSecretKey(participantID, newSecretKey, isInternalRequest);
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
            return dataAccess.GetFraudCases();
        }

        public IEnumerable<DocumentMappingModel> GetDocumentTypes()
        {
            return dataAccess.GetDocumentTypes();
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
                return dataAccess.InitParticipant(participantName, secretKey, ips);
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

        public IEnumerable<string> GetParticipantName(bool isInternalRequest)
        {
            var participantName = dataAccess.GetParticipantName(isInternalRequest);

            return participantName ?? new List<string>();
        }
    }
}