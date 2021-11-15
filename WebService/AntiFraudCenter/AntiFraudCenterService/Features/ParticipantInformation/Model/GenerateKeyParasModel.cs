namespace AntiFraudCenterService.Features.ParticipantInformation.Model
{
    public class GenerateKeyParasModel
    {
        public string EncryptedSecretKey { get; set; }

        public int? ParticipantID { get; set; }
    }
}
