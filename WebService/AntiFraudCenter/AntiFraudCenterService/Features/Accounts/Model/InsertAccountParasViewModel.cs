namespace AntiFraudCenterService.Features.FraudCustomer
{
    using System.Collections.Generic;

    public class InsertAccountParasViewModel
    {
        public string Key { get; set; }

        public IEnumerable<InsertAccountParasModel> Data { get; set; }
    }
}