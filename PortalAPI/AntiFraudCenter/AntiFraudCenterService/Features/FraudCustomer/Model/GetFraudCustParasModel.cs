namespace AntiFraudCenterService.Features.FraudCustomer
{
    using AntiFraudCenterService.Shares;
    using System.ComponentModel.DataAnnotations;

    public class GetFraudCustParasModel
    {
        public string AccountID { get; set; }

        [MaxLength(Constants.MaxLenght)]
        public string Phone { get; set; }

        public string CountryCode { get; set; }

        [MaxLength(Constants.MaxLenght)]
        public string Email { get; set; }

        [MaxLength(Constants.MaxLenght)]
        public string DocumentID { get; set; }

        public int? DocumentTypeID { get; set; }

        public string BankName { get; set; }

        [MaxLength(Constants.MaxLenght)]
        public string BankAccountID { get; set; }
    }
}