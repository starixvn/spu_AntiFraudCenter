namespace AntiFraudCenterService.Features.AccountMaterials
{
    using System.ComponentModel.DataAnnotations;

    public class DeleteIPParasModel
    {
        [Required]
        public string AccountID { get; set; }

        [Required]
        public string IP { get; set; }
    }
}