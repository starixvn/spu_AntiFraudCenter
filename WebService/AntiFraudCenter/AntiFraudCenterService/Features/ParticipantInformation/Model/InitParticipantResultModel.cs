namespace AntiFraudCenterService.Features.ParticipantInformation.Model
{
    using static AntiFraudCenterService.Shares.Constants;

    public class InitParticipantResultModel
    {
        public HttpStatus StatusCode { get; set; }

        public string ErrorMessage { get; set; }

        public InitParticipantModel Participant { get; set; }
    }
}