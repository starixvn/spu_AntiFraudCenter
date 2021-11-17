namespace AntiFraudCenterService.Features.FraudCustomer
{
    using System.Collections.Generic;

    public class DeleteFraudCustParasViewModel
    {
        public string Key { get; set; }

        public IEnumerable<DeleteFraudCustParasModel> Data { get; set; }
    }
}