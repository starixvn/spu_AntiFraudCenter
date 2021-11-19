using AntiFraudCenterService.Shares;
using AntiFraudCenterService.Shares.ValidationData;
using System.ComponentModel.DataAnnotations;

namespace AntiFraudCenterService.Features.FraudCustomer
{
    public class InsertAccountParasModel
    {
        public string AccountID { get; set; }

        public string FullName { get; set; }

        [MaxLength(Constants.MaxLenght)]
        [RequiredDependencyAttribute("CountryCode")]
        public string Phone { get; set; }

        [RequiredDependencyAttribute("Phone")]
        public string CountryCode { get; set; }

        [MaxLength(Constants.MaxLenght)]
        public string Email { get; set; }

        [MaxLength(Constants.MaxLenght)]
        [RequiredDependencyAttribute("DocumentTypeID")]
        public string DocumentID { get; set; }

        [RequiredDependencyAttribute("DocumentID")]
        public int DocumentTypeID { get; set; }

        [RequiredDependencyAttribute("BankAccountID")]
        public string BankName { get; set; }

        [MaxLength(Constants.MaxLenght)]
        [RequiredDependencyAttribute("BankName")]
        public string BankAccountID { get; set; }

        public int FraudID { get; set; }
    }
}