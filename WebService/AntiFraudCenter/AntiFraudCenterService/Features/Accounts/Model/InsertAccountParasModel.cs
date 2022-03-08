namespace AntiFraudCenterService.Features.Accounts
{
    using AntiFraudCenterService.Shares;
    using AntiFraudCenterService.Shares.ValidationData;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class InsertAccountParasModel
    {
        public string AccountID { get; set; }

        public string FullName { get; set; }

        [MaxLength(Constants.MaxLenght)]
        [RequiredDependency("CountryCode")]
        [RequiredAtLeast("Email", "DocumentID", "BankAccount")]
        public string Phone { get; set; }

        [RequiredDependency("Phone")]
        public string CountryCode { get; set; }

        [MaxLength(Constants.MaxLenght)]
        public string Email { get; set; }

        [MaxLength(Constants.MaxLenght)]
        [RequiredDependency("DocumentTypeID")]
        public string DocumentID { get; set; }

        [RequiredDependency("DocumentID")]
        public int? DocumentTypeID { get; set; }

        [JsonProperty(PropertyName = "BankAccount")]
        [RequiredDependency("BankAccount")]
        public IEnumerable<BankAccountModel> BankAccount { get; set; }

        [Required]
        public IEnumerable<int> FraudID { get; set; }
    }
}