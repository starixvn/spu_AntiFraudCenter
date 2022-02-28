namespace AntiFraudCenterService.Shares.ViewModel
{
    using AntiFraudCenterService.Features.Accounts;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using static AntiFraudCenterService.Shares.Constants;

    public class GetAccountResultViewModel
    {
        [JsonProperty(PropertyName = "StatusCode")]
        public HttpStatus StatusCode { get; set; }

        [JsonProperty(PropertyName = "ErrorMessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty(PropertyName = "Data")]
        public IEnumerable<GetAccountResultModel> Data { get; set; }
    }
}