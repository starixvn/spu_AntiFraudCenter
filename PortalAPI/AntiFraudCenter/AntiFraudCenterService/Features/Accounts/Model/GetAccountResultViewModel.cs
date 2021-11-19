namespace AntiFraudCenterService.Shares.ViewModel
{
    using AntiFraudCenterService.Features.FraudCustomer;
    using System.Collections.Generic;
    using static AntiFraudCenterService.Shares.Constants;

    public class GetAccountResultViewModel
    {
        public HttpStatus StatusCode { get; set; }

        public string ErrorMessage { get; set; }

        public IEnumerable<GetAccountResultModel> Data { get; set; }
    }
}