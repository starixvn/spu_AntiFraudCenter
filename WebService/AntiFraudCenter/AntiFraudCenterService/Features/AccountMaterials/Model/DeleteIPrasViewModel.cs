namespace AntiFraudCenterService.Features.AccountMaterials
{
    using System.Collections.Generic;

    public class DeleteIPrasViewModel
    {
        public string Key { get; set; }

        public IEnumerable<DeleteIPParasModel> Data { get; set; }
    }
}