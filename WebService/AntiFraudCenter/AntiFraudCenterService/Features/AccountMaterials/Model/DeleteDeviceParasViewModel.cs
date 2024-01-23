namespace AntiFraudCenterService.Features.AccountMaterials
{
    using System.Collections.Generic;

    public class DeleteDeviceParasViewModel
    {
        public string Key { get; set; }

        public IEnumerable<DeleteDeviceParasModel> Data { get; set; }
    }
}