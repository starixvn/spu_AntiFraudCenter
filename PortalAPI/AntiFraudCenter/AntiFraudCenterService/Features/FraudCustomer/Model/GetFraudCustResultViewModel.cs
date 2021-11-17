namespace AntiFraudCenterService.Shares.ViewModel
{
    using AntiFraudCenterService.Features.FraudCustomer;
    using System.Collections.Generic;
    using static AntiFraudCenterService.Shares.Constants;

    public class GetFraudCustResultViewModel
    {
        public HttpStatus StatusCode { get; set; }

        public string ErrorMessage { get; set; }

        public IEnumerable<GetFraudCustResultModel> Data { get; set; }
    }
}