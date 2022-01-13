namespace AntiFraudCenterService.Features.FraudCustomer
{
    using System;

    public class GetAccountByDateParasModel
    {
        public string ParticipantName { get; set; }

        public DateTime? DateFrom { get; set; } 

        public DateTime? DateTo { get; set; }
    }
}