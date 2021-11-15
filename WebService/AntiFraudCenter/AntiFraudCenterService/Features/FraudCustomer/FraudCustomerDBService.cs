namespace AntiFraudCenterService.Features.FraudCustomer
{
    using AntiFraudCenterService.Shares;
    using AntiFraudCenterService.Shares.ViewModel;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static AntiFraudCenterService.Shares.Constants;

    public interface IFraudCustomerDBService
    {
        IEnumerable<GetFraudCustomerModel> GetFraudCustomers(string encryptedData);

        ParticipantModel GetParticipant(int? participantID);

        string GetFraudCustomerDecryption(int? participantID);
    }

    public class FraudCustomerDBService : IFraudCustomerDBService
    {
        private readonly IFraudCustomerDataAccess fraudCustomerDataAccess;  

        public FraudCustomerDBService(IFraudCustomerDataAccess fraudCustomerDataAccess)
        {
            this.fraudCustomerDataAccess = fraudCustomerDataAccess;
        }

        public IEnumerable<GetFraudCustomerModel> GetFraudCustomers(string decryptedData)
        {
            if (!string.IsNullOrEmpty(decryptedData))
            {
                return fraudCustomerDataAccess.GetFraudCustomers(decryptedData) ?? new List<GetFraudCustomerModel>();
            }

            return new List<GetFraudCustomerModel>();
        }

        public ParticipantModel GetParticipant(int? participantID)
        {
            var participants =  fraudCustomerDataAccess.GetParticipant();

            if(participants != null && participants.Any())
            {
                return participants.SingleOrDefault(c => c.ParticipantID == participantID);
            }

            return null;
        }


        public string GetFraudCustomerDecryption(int? participantID)
        {
            if (participantID != null)
            {
                var participant = GetParticipant(participantID);

                if (participant != null)
                {
                    var dummy = new GetFraudCustomerModel
                    {
                        FullName = "abc"
                    };


                    return JsonConvert.SerializeObject(CryptographicHelper.AESEncrypt(JsonConvert.SerializeObject(dummy), participant.SecretKey, participant.ParticipantName));
                }
            }

            return string.Empty;
        }
    }
}