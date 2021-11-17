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

        IEnumerable<ParticipantModel> GetParticipant();

        ParticipantModel CheckAuthentication(string key);

        FraudCustDBResult AddFraudCustomers(string decryptedData, int? participantID);

        FraudCustDBResult UpdateFraudCustomers(string decryptedData, int? participantID);

        FraudCustDBResult DeleteFraudCustomers(string decryptedData, int? participantID);

        string EncryptedFieldsToInsertDB(string jsonData);

        string EncryptedFieldsToGetDB(string jsonData);

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

        public IEnumerable<ParticipantModel> GetParticipant()
        {
            var participants = fraudCustomerDataAccess.GetParticipant();

            return participants ?? new List<ParticipantModel>();
        }

        public ParticipantModel CheckAuthentication(string key)
        {
            var participants = GetParticipant();

            if (participants != null && participants.Any())
            {
                return participants.SingleOrDefault(c => string.Equals(c.SecretKey,key));
            }

            return null;
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

        public string EncryptedFieldsToGetDB(string jsonData)
        {
            if (!string.IsNullOrEmpty(jsonData))
            {
                var para = JsonConvert.DeserializeObject<GetFraudCustomerParasModel>(jsonData);

                if (para != null)
                {
                    para.Phone = CryptographicHelper.AESDecrypt(para.Phone, EncryptedKey, IVKey);
                    para.Email = CryptographicHelper.AESDecrypt(para.Email, EncryptedKey, IVKey);
                    para.DocumentID = CryptographicHelper.AESDecrypt(para.DocumentID, EncryptedKey, IVKey);
                    para.BankAccountID = CryptographicHelper.AESDecrypt(para.BankAccountID, EncryptedKey, IVKey);

                    return JsonConvert.SerializeObject(para);
                }
            }

            return string.Empty;
        }

        public string EncryptedFieldsToInsertDB(string jsonData)
        {
            if (!string.IsNullOrEmpty(jsonData))
            {
                var fraudCusts = JsonConvert.DeserializeObject<List<InsertFraudCustomerModel>>(jsonData);

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