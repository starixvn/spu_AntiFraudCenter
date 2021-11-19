namespace AntiFraudCenterService.Features.FraudCustomer
{
    using AntiFraudCenterService.Shares;
    using AntiFraudCenterService.Shares.Model;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using static AntiFraudCenterService.Shares.Constants;

    public interface IAccountDBService
    {
        IEnumerable<GetAccountResultModel> GetAccounts(GetAccountParasModel encryptedData);

        IEnumerable<ParticipantModel> GetParticipant();

        ParticipantModel CheckAuthentication(string key);

        AccountDBResult InsertAccounts(IEnumerable<InsertAccountParasModel> encryptedData, int? participantID);

        AccountDBResult UpdateAccounts(IEnumerable<InsertAccountParasModel> encryptedData, int? participantID);

        AccountDBResult DeleteAccounts(IEnumerable<DeleteAccountParasModel> accountIDs, int? participantID);

        IEnumerable<InsertAccountParasModel> EncryptedFieldsToInsertDB(IEnumerable<InsertAccountParasModel> data);

        GetAccountParasModel EncryptedFieldsToGetDB(GetAccountParasModel data);

        IEnumerable<GetAccountResultModel> DecryptedFieldsFromDB(IEnumerable<GetAccountResultModel> fraudCusts);
    }

    public class AccountDBService : IAccountDBService
    {
        private readonly IAccountDataAccess accountDataAccess;

        public AccountDBService(IAccountDataAccess accountDataAccess)
        {
            this.accountDataAccess = accountDataAccess;
        }

        public IEnumerable<GetAccountResultModel> GetAccounts(GetAccountParasModel encryptedData)
        {
            if (encryptedData != null)
            {
                return accountDataAccess.GetAccounts(encryptedData) ?? new List<GetAccountResultModel>();
            }

            return new List<GetAccountResultModel>();
        }

        public IEnumerable<ParticipantModel> GetParticipant()
        {
            var participants = accountDataAccess.GetParticipant();

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

        public AccountDBResult InsertAccounts(IEnumerable<InsertAccountParasModel> encryptedData, int? participantID)
        {
            if (encryptedData != null && encryptedData.Any() && participantID != null)
            {
                return accountDataAccess.AddAccounts(participantID, encryptedData);
            }

            return new AccountDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
        }

        public AccountDBResult UpdateAccounts(IEnumerable<InsertAccountParasModel> encryptedData, int? participantID)
        {
            if (encryptedData != null && participantID != null)
            {
                return accountDataAccess.UpdateAccounts(participantID, encryptedData);
            }

            return new AccountDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
        }

        public AccountDBResult DeleteAccounts(IEnumerable<DeleteAccountParasModel> accountIDs, int? participantID)
        {
            if (accountIDs != null && accountIDs.Any() && participantID != null)
            {
                return accountDataAccess.DeleteAccounts(participantID, accountIDs);
            }

            return new AccountDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
        }

        public GetAccountParasModel EncryptedFieldsToGetDB(GetAccountParasModel data)
        {
            if (data != null)
            {
                data.Phone = string.IsNullOrEmpty(data.Phone) ? null : CryptographicHelper.AESEncrypt(data.Phone, EncryptedKey, IVKey);
                data.Email = string.IsNullOrEmpty(data.Email) ? null : CryptographicHelper.AESEncrypt(data.Email, EncryptedKey, IVKey);
                data.DocumentID = string.IsNullOrEmpty(data.DocumentID) ? null : CryptographicHelper.AESEncrypt(data.DocumentID, EncryptedKey, IVKey);
                data.BankAccountID = string.IsNullOrEmpty(data.BankAccountID) ? null : CryptographicHelper.AESEncrypt(data.BankAccountID, EncryptedKey, IVKey);

                return data;
            }

            return new GetAccountParasModel();
        }

        public IEnumerable<InsertAccountParasModel> EncryptedFieldsToInsertDB(IEnumerable<InsertAccountParasModel> data)
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

            return new List<InsertAccountParasModel>();
        }

        public IEnumerable<GetAccountResultModel> DecryptedFieldsFromDB(IEnumerable<GetAccountResultModel> fraudCusts)
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

        public AccountDBResult VerifyInputData(string jsonData)
        {
            if (!string.IsNullOrEmpty(jsonData))
            {
                var fraudCusts = JsonConvert.DeserializeObject<List<InsertAccountParasModel>>(jsonData);

                if (fraudCusts.Any())
                {
                    foreach (var fraudCust in fraudCusts)
                    {
                        if (fraudCust.Phone.Length > MaxLenght
                            || fraudCust.Email.Length > MaxLenght
                            || fraudCust.DocumentID.Length > MaxLenght
                            || fraudCust.BankAccountID.Length > MaxLenght)
                        {
                            return new AccountDBResult { StatusCode = HttpStatus.BadRequest, ErrorMessage = string.Empty };
                        }
                    }
                }
            }

            return new AccountDBResult { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty };
        }
    }
}