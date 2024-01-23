namespace AntiFraudCenterService.Features.AccountMaterials
{
    using System.ComponentModel.DataAnnotations;

    public class InsertDeviceParasModel
    {
        [Required]
        public string AccountID { get; set; }

        [Required]
        public string Device { get; set; }
    }
}