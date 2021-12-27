namespace AntiFraudCenterService.Shares.ViewModel
{
    using AntiFraudCenterService.Features.FraudCustomer;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using static AntiFraudCenterService.Shares.Constants;

    public class GetAccountByDateResultViewModel
    {
        [JsonProperty(PropertyName = "StatusCode")]
        public HttpStatus StatusCode { get; set; }

        [JsonProperty(PropertyName = "ErrorMessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty(PropertyName = "Data")]
        public GetAccountByDateResultModel Data { get; set; }
    }
}