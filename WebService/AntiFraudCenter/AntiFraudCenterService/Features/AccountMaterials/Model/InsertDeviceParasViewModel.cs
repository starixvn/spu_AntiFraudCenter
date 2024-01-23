namespace AntiFraudCenterService.Features.AccountMaterials
{
    using System.Collections.Generic;

    public class InsertDeviceParasViewModel
    {
        public string Key { get; set; }

        public IEnumerable<InsertDeviceParasModel> Data { get; set; }
    }
}