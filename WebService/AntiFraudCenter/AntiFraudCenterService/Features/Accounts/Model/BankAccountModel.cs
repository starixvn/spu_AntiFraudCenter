namespace AntiFraudCenterService.Features.Accounts
{
    using AntiFraudCenterService.Shares;
    using AntiFraudCenterService.Shares.ValidationData;
    using System.ComponentModel.DataAnnotations;

    public class BankAccountModel
    {
        [RequiredDependency("BankName")]
        public string BankName { get; set; }

        [RequiredDependency("BankAccountID")]
        [MaxLength(Constants.MaxLenght)]
        public string BankAccountID { get; set; }
    }
}