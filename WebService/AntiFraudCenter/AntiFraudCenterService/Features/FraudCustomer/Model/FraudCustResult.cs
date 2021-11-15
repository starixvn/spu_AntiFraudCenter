namespace AntiFraudCenterService.Shares.ViewModel
{
    using AntiFraudCenterService.Features.FraudCustomer;
    using System.Collections.Generic;
    using static AntiFraudCenterService.Shares.Constants;

    public class FraudCustResult
    {
        public HttpStatus ErrorCode { get; set; }

        public string ErrorMsg { get; set; }

        public IEnumerable<GetFraudCustomerModel> FraudCusts { get; set; }
    }
}