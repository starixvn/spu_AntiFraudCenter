namespace AntiFraudCenterService.Features.FraudCustomer
{
    using AntiFraudCenterService.Features.FraudCustomer.Criteria;
    using AntiFraudCenterService.Shares.Model;
    using Fanex.Data.Repository;
    using static AntiFraudCenterService.Shares.Constants;

    public interface IParticipantInfoDataAccess
    {
        DBResult UpdateParticipantSecretKey(int? participantID, string secretKey);
    }

    public class ParticipantInfoDataAccess : IParticipantInfoDataAccess
    {
        private readonly IDynamicRepository repository;

        public ParticipantInfoDataAccess()
        {
            repository = new DynamicRepository();
        }

        public DBResult UpdateParticipantSecretKey(int? participantID, string secretKey)
        {
            var criteria = new ParticipantSecretKeyUpdateCriteria
            {
                ParticipantID = participantID,
                SecretKey = secretKey
            };

            repository.Execute(criteria);

            return new DBResult
            {
                ErrorCode = HttpStatus.OK,
                ErrorMsg = string.Empty
            };
        }
    }
}