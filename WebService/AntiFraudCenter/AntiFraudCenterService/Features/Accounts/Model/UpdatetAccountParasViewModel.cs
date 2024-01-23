namespace AntiFraudCenterService.Features.Accounts
{
    using System.Collections.Generic;

    public class UpdatetAccountParasViewModel
    {
        public string Key { get; set; }

        public IEnumerable<UpdateAccountParasModel> Data { get; set; }
    }
}