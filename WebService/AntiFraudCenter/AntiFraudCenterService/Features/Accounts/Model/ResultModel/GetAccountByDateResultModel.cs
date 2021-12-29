namespace AntiFraudCenterService.Features.FraudCustomer
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public class GetAccountByDateResultModel
    {
        [JsonProperty(PropertyName = "IsContinue")]
        public bool IsContinue { get; set; }

        [JsonProperty(PropertyName = "DateFrom")]
        public DateTime? DateFrom { get; set; }

        [JsonProperty(PropertyName = "DataTo")]
        public DateTime? DateTo { get; set; }

        [JsonProperty(PropertyName = "Data")]
        public IEnumerable<GetAccountResultModel> Data { get; set; }
    }
}