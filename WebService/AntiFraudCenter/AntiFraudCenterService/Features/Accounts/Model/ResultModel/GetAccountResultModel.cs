using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace AntiFraudCenterService.Features.FraudCustomer
{
    public class GetAccountResultModel
    {
        [JsonProperty(PropertyName = "AccountID")]
        public string AccountID { get; set; }

        [JsonProperty(PropertyName = "FullName")]
        public string FullName { get; set; }

        [JsonProperty(PropertyName = "ParticipantName")]
        public string ParticipantName { get; set; }

        [JsonProperty(PropertyName = "Phone")]
        public string Phone { get; set; }

        [JsonProperty(PropertyName = "CountryCode")]
        public string CountryCode { get; set; }

        [JsonProperty(PropertyName = "Email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "DocumentID")]
        public string DocumentID { get; set; }

        [JsonProperty(PropertyName = "DocumentTypeID")]
        public int DocumentTypeID { get; set; }

        [JsonProperty(PropertyName = "BankName")]
        public string BankName { get; set; }

        [JsonProperty(PropertyName = "BankAccountID")]
        public string BankAccountID { get; set; }

        [IgnoreDataMember]
        public string FraudID { get; set; }

        [IgnoreDataMember]
        public string Modified { get; set; }

        [IgnoreDataMember]
        public string Created { get; set; }

        [JsonProperty(PropertyName = "FraudID")]
        public IEnumerable<int> FraudIDList
        {
            get
            {
                if (!string.IsNullOrEmpty(FraudID))
                {
                    return FraudID.Split(",").Select(int.Parse);
                }

                return new List<int>();
            }
        }
    }
}