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
        IEnumerable<GetFraudCustResultModel> GetFraudCustomers(GetFraudCustParasModel encryptedData);

        IEnumerable<ParticipantModel> GetParticipant();

        ParticipantModel CheckAuthentication(string key);

        FraudCustDBResult AddFraudCustomers(IEnumerable<InsertFraudCustParasModel> encryptedData, int? participantID);

        FraudCustDBResult UpdateFraudCustomers(IEnumerable<InsertFraudCustParasModel> encryptedData, int? participantID);

        FraudCustDBResult DeleteFraudCustomers(IEnumerable<DeleteFraudCustParasModel> accountIDs, int? participantID);

        IEnumerable<InsertFraudCustParasModel> EncryptedFieldsToInsertDB(IEnumerable<InsertFraudCustParasModel> data);

        GetFraudCustParasModel EncryptedFieldsToGetDB(GetFraudCustParasModel data);

        IEnumerable<GetFraudCustResultModel> DecryptedFieldsFromDB(IEnumerable<GetFraudCustResultModel> fraudCusts);
    }

    public class FraudCustomerDBService : IFraudCustomerDBService
    {
        private readonly IFraudCustomerDataAccess fraudCustomerDataAccess;

        public FraudCustomerDBService(IFraudCustomerDataAccess fraudCustomerDataAccess)
        {
            this.fraudCustomerDataAccess = fraudCustomerDataAccess;
        }

        public IEnumerable<GetFraudCustResultModel> GetFraudCustomers(GetFraudCustParasModel encryptedData)
        {
            if (encryptedData != null)
            {
                return fraudCustomerDataAccess.GetFraudCustomers(encryptedData) ?? new List<GetFraudCustResultModel>();
            }

            return new List<GetFraudCustResultModel>();
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
                return participants.SingleOrDefault(c => string.Equals(c.SecretKey, key));
            }

            return null;
        }

        public FraudCustDBResult AddFraudCustomers(IEnumerable<InsertFraudCustParasModel> encryptedData, int? participantID)
        {
            if (encryptedData != null && encryptedData.Any() && participantID != null)
            {
                return fraudCustomerDataAccess.AddFraudCustomers(participantID, encryptedData);
            }

            return new FraudCustDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
        }

        public FraudCustDBResult UpdateFraudCustomers(IEnumerable<InsertFraudCustParasModel> encryptedData, int? participantID)
        {
            if (encryptedData != null && participantID != null)
            {
                return fraudCustomerDataAccess.UpdateFraudCustomers(participantID, encryptedData);
            }

            return new FraudCustDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
        }

        public FraudCustDBResult DeleteFraudCustomers(IEnumerable<DeleteFraudCustParasModel> accountIDs, int? participantID)
        {
            if (accountIDs != null && accountIDs.Any() && participantID != null)
            {
                return fraudCustomerDataAccess.DeleteFraudCustomers(participantID, accountIDs);
            }

            return new FraudCustDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
        }

        public GetFraudCustParasModel EncryptedFieldsToGetDB(GetFraudCustParasModel data)
        {
            if (data != null)
            {
                data.Phone = string.IsNullOrEmpty(data.Phone)? null : CryptographicHelper.AESEncrypt(data.Phone, EncryptedKey, IVKey);
                data.Email = string.IsNullOrEmpty(data.Email) ? null : CryptographicHelper.AESEncrypt(data.Email, EncryptedKey, IVKey);
                data.DocumentID = string.IsNullOrEmpty(data.DocumentID) ? null : CryptographicHelper.AESEncrypt(data.DocumentID, EncryptedKey, IVKey);
                data.BankAccountID = string.IsNullOrEmpty(data.BankAccountID) ? null : CryptographicHelper.AESEncrypt(data.BankAccountID, EncryptedKey, IVKey);

                return data;
            }

            return new GetFraudCustParasModel();
        }

        public IEnumerable<InsertFraudCustParasModel> EncryptedFieldsToInsertDB(IEnumerable<InsertFraudCustParasModel> data)
        {
            if (data != null && data.Any())
            {

                data.ToList().ForEach(c =>
                    {
                        c.Phone = string.IsNullOrEmpty(c.Phone) ? null : CryptographicHelper.AESEncrypt(c.Phone, EncryptedKey, IVKey);
                        c.Email = string.IsNullOrEmpty(c.Email) ? null : CryptographicHelper.AESEncrypt(c.Email, EncryptedKey, IVKey);
                        c.DocumentID = string.IsNullOrEmpty(c.DocumentID) ? null : CryptographicHelper.AESEncrypt(c.DocumentID, EncryptedKey, IVKey);
                        c.BankAccountID = string.IsNullOrEmpty(c.BankAccountID) ? null : CryptographicHelper.AESEncrypt(c.BankAccountID, EncryptedKey, IVKey);
                    });

                return data;
                }


            return new List<InsertFraudCustParasModel>();
        }

        public IEnumerable<GetFraudCustResultModel> DecryptedFieldsFromDB(IEnumerable<GetFraudCustResultModel> fraudCusts)
        {
            if (fraudCusts != null && fraudCusts.Any())
            {
                fraudCusts.ToList().ForEach(c =>
                {
                    c.Phone = string.IsNullOrEmpty(c.Phone) ? null : CryptographicHelper.AESDecrypt(c.Phone, EncryptedKey, IVKey);
                    c.Email = string.IsNullOrEmpty(c.Email) ? null : CryptographicHelper.AESDecrypt(c.Email, EncryptedKey, IVKey);
                    c.DocumentID = string.IsNullOrEmpty(c.DocumentID) ? null : CryptographicHelper.AESDecrypt(c.DocumentID, EncryptedKey, IVKey);
                    c.BankAccountID = string.IsNullOrEmpty(c.BankAccountID) ? null : CryptographicHelper.AESDecrypt(c.BankAccountID, EncryptedKey, IVKey);
                });
            }

            return fraudCusts;
        }

        public FraudCustDBResult VerifyInputData(string jsonData)
        {
            if (!string.IsNullOrEmpty(jsonData))
            {
                var fraudCusts = JsonConvert.DeserializeObject<List<InsertFraudCustParasModel>>(jsonData);

                if (fraudCusts.Any())
                {
                    foreach(var fraudCust in fraudCusts)
                    {
                        if (fraudCust.Phone.Length> MaxLenght
                            || fraudCust.Email.Length > MaxLenght
                            || fraudCust.DocumentID.Length > MaxLenght
                            || fraudCust.BankAccountID.Length > MaxLenght)
                        {
                            return new FraudCustDBResult { StatusCode = HttpStatus.BadRequest, ErrorMessage = string.Empty };
                        }
                    }
                }
            }

            return new FraudCustDBResult { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty };
        }
    }
}