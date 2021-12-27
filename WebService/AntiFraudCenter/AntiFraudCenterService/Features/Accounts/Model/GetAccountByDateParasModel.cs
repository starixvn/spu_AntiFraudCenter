namespace AntiFraudCenterService.Features.FraudCustomer
{
    using System;

    public class GetAccountByDateParasModel
    {
        public string ParticipantName { get; set; }

        public DateTime? DataFrom { get; set; }

        public DateTime? DataTo { get; set; }
    }
}