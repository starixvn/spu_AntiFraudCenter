namespace AntiFraudCenterService.Features.Accounts
{
    using System.Collections.Generic;

    public class DeleteAccountParasViewModel
    {
        public string Key { get; set; }

        public IEnumerable<DeleteAccountParasModel> Data { get; set; }
    }
}