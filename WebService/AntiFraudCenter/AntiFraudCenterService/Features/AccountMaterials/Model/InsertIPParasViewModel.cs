namespace AntiFraudCenterService.Features.AccountMaterials
{
    using System.Collections.Generic;

    public class InsertIPParasViewModel
    {
        public string Key { get; set; }

        public IEnumerable<InsertIPParasModel> Data { get; set; }
    }
}