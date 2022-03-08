namespace AntiFraudCenterService.Features.Accounts
{
    using AntiFraudCenterService.Shares;
    using AntiFraudCenterService.Shares.Model;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using static AntiFraudCenterService.Shares.Constants;
    using static AntiFraudCenterService.Shares.CryptographicConfig;

    public interface IAccountDBService
    {
        IEnumerable<GetAccountResultModel> GetAccounts(GetAccountParasModel encryptedData, bool isInternalRequest);

        GetAccountByDateResultModel GetAccountsByDate(GetAccountByDateParasModel para, int participantId, bool isInternalRequest);

        IEnumerable<ParticipantModel> GetParticipant(bool isInternalRequest);

        ParticipantModel CheckAuthentication(string key, bool isInternalRequest);

        AccountDBResult InsertAccounts(IEnumerable<InsertAccountModel> encryptedData, int? participantID, bool isInternalRequest, int batchSize);

        AccountDBResult UpdateAccounts(IEnumerable<InsertAccountModel> encryptedData, int? participantID, bool isInternalRequest, int batchSize);

        AccountDBResult DeleteAccounts(IEnumerable<DeleteAccountParasModel> accountIDs, int? participantID, bool isInternalRequest, int batchSize);

        IEnumerable<InsertAccountModel> EncryptAccount(IEnumerable<InsertAccountParasModel> data);

        GetAccountParasModel EncryptAccount(GetAccountParasModel data);

        IEnumerable<GetAccountResultModel> DecryptAccount(IEnumerable<GetAccountResultModel> fraudCusts);

        AccountDBResult CheckDataValidation(IEnumerable<AccountValidationParaModel> data, int? participantID, RequestAction action, bool isInternalRequest);

        void LogErrorToDB(int? participantID, string errMessage, string dataJson, bool isInternalRequest);
    }

    public class AccountDBService : IAccountDBService
    {
        private readonly IAccountDataAccess accountDataAccess;

        public AccountDBService(IAccountDataAccess accountDataAccess)
        {
            this.accountDataAccess = accountDataAccess;
        }

        public IEnumerable<GetAccountResultModel> GetAccounts(GetAccountParasModel encryptedData, bool isInternalRequest)
        {
            if (encryptedData != null)
            {
                return accountDataAccess.GetAccounts(encryptedData, isInternalRequest) ?? new List<GetAccountResultModel>();
            }

            return new List<GetAccountResultModel>();
        }

        public GetAccountByDateResultModel GetAccountsByDate(GetAccountByDateParasModel para, int participantId, bool isInternalRequest)
        {
            if (para != null)
            {
                return accountDataAccess.GetAccountByDate(para, participantId, isInternalRequest);
            }

            return new GetAccountByDateResultModel
            {
                IsContinue = false,
                DateFrom = null,
                DateTo = null,
                Data = Enumerable.Empty<GetAccountResultModel>()
            };
        }

        public IEnumerable<ParticipantModel> GetParticipant(bool isInternalRequest)
        {
            var participants = accountDataAccess.GetParticipant(isInternalRequest);

            return participants ?? new List<ParticipantModel>();
        }

        public ParticipantModel CheckAuthentication(string key, bool isInternalRequest)
        {
            var participants = GetParticipant(isInternalRequest);

            if (participants != null && participants.Any())
            {
                return participants.SingleOrDefault(c => string.Equals(c.SecretKey, key));
            }

            return null;
        }

        public AccountDBResult InsertAccounts(IEnumerable<InsertAccountModel> encryptedData, int? participantID, bool isInternalRequest, int batchSize)
        {
            if (encryptedData != null && encryptedData.Any() && participantID != null)
            {
                var dataChunk = encryptedData.ChunkBy(batchSize);

                foreach (var data in dataChunk)
                {
                    var result = accountDataAccess.InsertAccounts(participantID, encryptedData, isInternalRequest);

                    if (!string.IsNullOrEmpty(result.ErrorMessage))
                    {
                        LogErrorToDB(participantID, result.ErrorMessage, JsonConvert.SerializeObject(data), isInternalRequest);
                    }
                }

                return new AccountDBResult() { StatusCode = HttpStatus.Created, ErrorMessage = string.Empty };
            }

            return new AccountDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
        }

        public AccountDBResult UpdateAccounts(IEnumerable<InsertAccountModel> encryptedData, int? participantID, bool isInternalRequest, int batchSize)
        {
            if (encryptedData != null && participantID != null)
            {
                var dataChunk = encryptedData.ChunkBy(batchSize);

                foreach (var data in dataChunk)
                {
                    var result = accountDataAccess.UpdateAccounts(participantID, encryptedData, isInternalRequest);

                    if (!string.IsNullOrEmpty(result.ErrorMessage))
                    {
                        LogErrorToDB(participantID, result.ErrorMessage, JsonConvert.SerializeObject(data), isInternalRequest);
                    }
                }

                return new AccountDBResult() { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty };
            }

            return new AccountDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
        }

        public AccountDBResult DeleteAccounts(IEnumerable<DeleteAccountParasModel> accountIDs, int? participantID, bool isInternalRequest, int batchSize)
        {
            if (accountIDs != null && accountIDs.Any() && participantID != null)
            {
                var dataChunk = accountIDs.ChunkBy(batchSize);

                foreach (var data in dataChunk)
                {
                    var result = accountDataAccess.DeleteAccounts(participantID, accountIDs, isInternalRequest);

                    if (!string.IsNullOrEmpty(result.ErrorMessage))
                    {
                        LogErrorToDB(participantID, result.ErrorMessage, JsonConvert.SerializeObject(data), isInternalRequest);
                    }
                }

                return new AccountDBResult() { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty };
            }
            return new AccountDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
        }

        public GetAccountParasModel EncryptAccount(GetAccountParasModel data)
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

        public IEnumerable<InsertAccountModel> EncryptAccount(IEnumerable<InsertAccountParasModel> data)
        {
            if (data != null && data.Any())
            {
                var result = new List<InsertAccountModel>();

                data.ToList().ForEach(c =>
                    {
                        result.Add(new InsertAccountModel
                        {
                            B2CCustId = c.AccountID,
                            FullName = c.FullName,
                            CountryCode = c.CountryCode,
                            Email = string.IsNullOrEmpty(c.Email) ? null : CryptographicHelper.AESEncrypt(c.Email, EncryptedKey, IVKey),
                            Phone = string.IsNullOrEmpty(c.Phone) ? null : CryptographicHelper.AESEncrypt(c.Phone, EncryptedKey, IVKey),
                            DocumentID = string.IsNullOrEmpty(c.DocumentID) ? null : CryptographicHelper.AESEncrypt(c.DocumentID, EncryptedKey, IVKey),
                            DocumentTypeID = c.DocumentTypeID,
                            BankAccount = EncryptBankAccount(c.BankAccount),
                            FraudID = c.FraudID
                        });
                    });

                return result;
            }

            return Enumerable.Empty<InsertAccountModel>();
        }

        public IEnumerable<GetAccountResultModel> DecryptAccount(IEnumerable<GetAccountResultModel> fraudCusts)
        {
            if (fraudCusts != null && fraudCusts.Any())
            {
                fraudCusts.ToList().ForEach(c =>
                {
                    c.Phone = string.IsNullOrEmpty(c.Phone) ? null : CryptographicHelper.AESDecrypt(c.Phone, EncryptedKey, IVKey);
                    c.Email = string.IsNullOrEmpty(c.Email) ? null : CryptographicHelper.AESDecrypt(c.Email, EncryptedKey, IVKey);
                    c.DocumentID = string.IsNullOrEmpty(c.DocumentID) ? null : CryptographicHelper.AESDecrypt(c.DocumentID, EncryptedKey, IVKey);
                    c.BankAccount = DecryptBankAccount(c.BankInfo);
                });
            }

            return fraudCusts;
        }

        public AccountDBResult CheckDataValidation(IEnumerable<AccountValidationParaModel> data, int? participantID, RequestAction action, bool isInternalRequest)
        {
            var result = new AccountDBResult { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty };

            if (data != null && data.Any())
            {
                if (action == RequestAction.Insert || action == RequestAction.Update)
                {
                    result = IsCountryCodeValid(data);
                }

                if (result.StatusCode == HttpStatus.OK && (action == RequestAction.Update || action == RequestAction.Delete))
                {
                    result = IsAccountValid(data, participantID, isInternalRequest);
                }
            }

            return result;
        }

        public void LogErrorToDB(int? participantID, string errMessage, string dataJson, bool isInternalRequest)
        {
            accountDataAccess.InsertErrorLog(participantID, errMessage, dataJson, isInternalRequest);
        }

        #region Private methods

        private AccountDBResult IsAccountValid(IEnumerable<AccountValidationParaModel> data, int? participantID, bool isInternalRequest)
        {
            if (data != null && data.Any())
            {
                var accountList = data.Select(c => new DeleteAccountParasModel { AccountID = c.AccountID });

                var result = accountDataAccess.CheckAccountValidation(participantID, accountList, isInternalRequest);

                return MappingErrorValidaion(result);
            }

            return new AccountDBResult { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty };
        }

        private AccountDBResult MappingErrorValidaion(IEnumerable<CheckAccountModel> checkAccountModels)
        {
            var notPermissionList = new List<string>();
            var notFoundList = new List<string>();

            if (checkAccountModels != null && checkAccountModels.Any())
            {
                checkAccountModels.ToList().ForEach(account =>
                {
                    if (account.Status.Contains("No Permission Account"))
                    {
                        notPermissionList.Add(account.AccountID);
                    }
                    else if (account.Status.Contains("Not Exist"))
                    {
                        notFoundList.Add(account.AccountID);
                    }
                });
            }

            var errorMessage = notPermissionList.Any() ? string.Format($"{NoPermissionAccount} : {{{{{string.Join(", ", notPermissionList)}}}}}; ") : string.Empty;
            errorMessage += notFoundList.Any() ? string.Format($" {NotExist} : {{{{{string.Join(", ", notFoundList)}}}}}") : string.Empty;

            if ((notPermissionList.Any() && notFoundList.Any()) || notPermissionList.Any())
            {
                return new AccountDBResult { StatusCode = HttpStatus.BadRequest, ErrorMessage = errorMessage };
            }

            if (notFoundList.Any())
            {
                return new AccountDBResult { StatusCode = HttpStatus.NotFound, ErrorMessage = errorMessage };
            }

            return new AccountDBResult { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty };
        }

        private AccountDBResult IsCountryCodeValid(IEnumerable<AccountValidationParaModel> data)
        {
            var invalidList = new List<string>();

            if (data != null && data.Any())
            {
                foreach (var each in data.ToList())
                {
                    if (!string.IsNullOrEmpty(each.Phone))
                    {
                        bool isValid = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                                  .Select(culture => new RegionInfo(culture.LCID))
                                  .Any(region => string.Equals(region.ThreeLetterISORegionName, each.CountryCode));

                        if (!isValid)
                        {
                            invalidList.Add(each.CountryCode);
                        }
                    }
                }
            }

            var errorMessage = invalidList.Any() ? string.Format($"{CountryCodeInvalid} : {{{{{string.Join(", ", invalidList.Distinct())}}}}}") : string.Empty;

            return invalidList.Any() ? new AccountDBResult { StatusCode = HttpStatus.BadRequest, ErrorMessage = errorMessage }
                    : new AccountDBResult { StatusCode = HttpStatus.OK, ErrorMessage = errorMessage };
        }

        private IEnumerable<BankAccountModel> EncryptBankAccount(IEnumerable<BankAccountModel> bankAccount)
        {
            if (bankAccount != null && bankAccount.Any())
            {
                bankAccount.ToList().ForEach(c =>
                {
                    c.BankAccountID = CryptographicHelper.AESEncrypt(c.BankAccountID, EncryptedKey, IVKey);
                });

                return bankAccount;
            }

            return Enumerable.Empty<BankAccountModel>();
        }

        private IEnumerable<BankAccountModel> DecryptBankAccount(string bankAccountJson)
        {
            if (!string.IsNullOrEmpty(bankAccountJson))
            {
                var bankAccounts = JsonConvert.DeserializeObject<IEnumerable<BankAccountModel>>(bankAccountJson);

                if (bankAccounts != null && bankAccounts.Any())
                {
                    bankAccounts.ToList().ForEach(c =>
                    {
                        c.BankAccountID = CryptographicHelper.AESDecrypt(c.BankAccountID, EncryptedKey, IVKey);
                    });
                }

                return bankAccounts;
            }

            return Enumerable.Empty<BankAccountModel>();
        }

        #endregion Private methods
    }
}