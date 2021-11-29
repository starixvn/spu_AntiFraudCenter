namespace AntiFraudCenterService.Features.ParticipantInformation.Model
{
    public class CreateParticipantParaModel
    {
        public string Password { get; set; }

        public string ParticipantName { get; set; }

        public string IPs { get; set; }
    }
}