namespace AntiFraudCenterService.Features.AccountMaterials
{
    using System.ComponentModel.DataAnnotations;

    public class DeleteDeviceParasModel
    {
        [Required]
        public string AccountID { get; set; }

        [Required]
        public string Device { get; set; }
    }
}