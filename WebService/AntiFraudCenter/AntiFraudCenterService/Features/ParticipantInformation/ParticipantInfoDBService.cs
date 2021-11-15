namespace AntiFraudCenterService.Features.FraudCustomer
{
    using AntiFraudCenterService.Shares.Model;
    using static AntiFraudCenterService.Shares.Constants;

    public interface IParticipantInfoDBService
    {
        DBResult UpdateParticipantSecretKey(int? participantID, string newSecretKey);
    }

    public class ParticipantInfoDBService : IParticipantInfoDBService
    {
        private readonly IParticipantInfoDataAccess participantInfoDataAccess;

        public ParticipantInfoDBService(IParticipantInfoDataAccess participantInfoDataAccess)
        {
            this.participantInfoDataAccess = participantInfoDataAccess;
        }

        public DBResult UpdateParticipantSecretKey(int? participantID, string newSecretKey)
        {
            if (!string.IsNullOrEmpty(newSecretKey) && participantID != null)
            {
                var rs = participantInfoDataAccess.UpdateParticipantSecretKey(participantID, newSecretKey);

                return new DBResult
                {
                    ErrorCode = string.IsNullOrEmpty(rs.ErrorMsg) ? HttpStatus.OK : HttpStatus.ServerNotImplemented,
                    ErrorMsg = rs.ErrorMsg
                };
            }

            return new DBResult
            {
                ErrorCode = HttpStatus.BadRequest,
                ErrorMsg = "Data input invalid"
            };
        }
    }
}