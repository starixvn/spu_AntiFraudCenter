namespace AntiFraudCenterService.Features.FraudCustomer
{
    using AntiFraudCenterService.Shares;
    using AntiFraudCenterService.Shares.Model;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using static AntiFraudCenterService.Shares.Constants;

    public interface IFraudCustomerDBService
    {
        IEnumerable<GetFraudCustomerModel> GetFraudCustomers(string decryptedData);

        ParticipantModel GetParticipant(int? participantID);

        string GetFraudCustomerDecryption(int? participantID);

        FraudCustDBResult AddFraudCustomers(string decryptedData, int? participantID);

        FraudCustDBResult UpdateFraudCustomers(string decryptedData, int? participantID);

        FraudCustDBResult DeleteFraudCustomers(string decryptedData, int? participantID);

        string EncryptedFieldsToInsertDB(string decryptedData);

        IEnumerable<GetFraudCustomerModel> DecryptedFieldsFromDB(IEnumerable<GetFraudCustomerModel> fraudCusts);
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
            var participants = fraudCustomerDataAccess.GetParticipant();

            if (participants != null && participants.Any())
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
                        AccountID = "AccountID_Vaule_Test",
                        FullName = "FullName_Vaule_Test",
                        Phone = "Phone_Vaule_Test",
                        CountryCode = "CountryCode_Vaule_Test",
                        Email = "Email_Vaule_Test",
                        DocumentID = "DocumentID_Vaule_Test",
                        DocumentTypeID = 1,
                        BankName = "BankName_Vaule_Test",
                        BankAccountID = "BankAccountID_Vaule_Test",
                        FraudID = 2
                    };

                    var dummy2 = new List<GetFraudCustomerModel>
                    {
                        new GetFraudCustomerModel
                        {
                            AccountID = "AccountID_Vaule_Test",
                            FullName = "FullName_Vaule_Test",
                            Phone = "Phone_Vaule_Test",
                            CountryCode = "CountryCode_Vaule_Test",
                            Email = "Email_Vaule_Test",
                            DocumentID = "DocumentID_Vaule_Test",
                            DocumentTypeID = 1,
                            BankName = "BankName_Vaule_Test",
                            BankAccountID = "BankAccountID_Vaule_Test",
                            FraudID = 2
                        },
                        dummy
                    };

                    string json = JsonConvert.SerializeObject(dummy2);

                    return CryptographicHelper.AESEncrypt(json, participant.SecretKey, participant.ParticipantName);
                }
            }

            return string.Empty;
        }

        public FraudCustDBResult AddFraudCustomers(string decryptedData, int? participantID)
        {
            if (!string.IsNullOrEmpty(decryptedData) && participantID != null)
            {
                return fraudCustomerDataAccess.AddFraudCustomers(participantID, decryptedData);
            }

            return new FraudCustDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
        }

        public FraudCustDBResult UpdateFraudCustomers(string decryptedData, int? participantID)
        {
            if (!string.IsNullOrEmpty(decryptedData) && participantID != null)
            {
                return fraudCustomerDataAccess.UpdateFraudCustomers(participantID, decryptedData);
            }

            return new FraudCustDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
        }

        public FraudCustDBResult DeleteFraudCustomers(string decryptedData, int? participantID)
        {
            if (!string.IsNullOrEmpty(decryptedData) && participantID != null)
            {
                return fraudCustomerDataAccess.DeleteFraudCustomers(participantID, decryptedData);
            }

            return new FraudCustDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
        }

        public string EncryptedFieldsToInsertDB(string decryptedData)
        {
            if (!string.IsNullOrEmpty(decryptedData))
            {
                var fraudCusts = JsonConvert.DeserializeObject<List<InsertFraudCustomerModel>>(decryptedData);

                if (fraudCusts.Any())
                {
                    fraudCusts.ForEach(c =>
                    {
                        c.Phone = CryptographicHelper.AESDecrypt(c.Phone, EncryptedKey, IVKey);
                        c.Email = CryptographicHelper.AESDecrypt(c.Email, EncryptedKey, IVKey);
                        c.DocumentID = CryptographicHelper.AESDecrypt(c.DocumentID, EncryptedKey, IVKey);
                        c.BankAccountID = CryptographicHelper.AESDecrypt(c.BankAccountID, EncryptedKey, IVKey);
                    });

                    return JsonConvert.SerializeObject(fraudCusts);
                }
            }

            return string.Empty;
        }

        public IEnumerable<GetFraudCustomerModel> DecryptedFieldsFromDB(IEnumerable<GetFraudCustomerModel> fraudCusts)
        {
            if (fraudCusts != null && fraudCusts.Any())
            {
                fraudCusts.ToList().ForEach(c =>
                {
                    c.Phone = CryptographicHelper.AESEncrypt(c.Phone, EncryptedKey, IVKey);
                    c.Email = CryptographicHelper.AESEncrypt(c.Email, EncryptedKey, IVKey);
                    c.DocumentID = CryptographicHelper.AESEncrypt(c.DocumentID, EncryptedKey, IVKey);
                    c.BankAccountID = CryptographicHelper.AESEncrypt(c.BankAccountID, EncryptedKey, IVKey);
                });
            }

            return fraudCusts;
        }
    }
}