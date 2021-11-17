namespace AntiFraudCenterService.Features.FraudCustomer
{
    using System.Collections.Generic;

    public class InsertFraudCustParasViewModel
    {
        public string Key { get; set; }

        public IEnumerable<InsertFraudCustParasModel> Data { get; set; }
    }
}