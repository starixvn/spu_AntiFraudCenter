namespace AntiFraudCenterService.Features.FraudCustomer
{
    using AntiFraudCenterService.Features.FraudCustomer.Criteria;
    using AntiFraudCenterService.Shares.Model;
    using Fanex.Data.Repository;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using static AntiFraudCenterService.Shares.Constants;

    public interface IAccountDataAccess
    {
        IEnumerable<GetAccountResultModel> GetAccounts(GetAccountParasModel encryptedData, bool isInternalRequest);

        IEnumerable<ParticipantModel> GetParticipant(bool isInternalRequest);

        AccountDBResult AddAccounts(int? participantID, IEnumerable<InsertAccountParasModel> encryptedData, bool isInternalRequest);

        AccountDBResult UpdateAccounts(int? participantID, IEnumerable<InsertAccountParasModel> encryptedData, bool isInternalRequest);

        AccountDBResult DeleteAccounts(int? participantID, IEnumerable<DeleteAccountParasModel> accountIDs, bool isInternalRequest);

        IEnumerable<CheckAccountModel> CheckAccountValidation(int? participantID, IEnumerable<DeleteAccountParasModel> accountIDs, bool isInternalRequest);
    }

    public class AccountDataAccess : IAccountDataAccess
    {
        private readonly IDynamicRepository repository;

        public AccountDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<GetAccountResultModel> GetAccounts(GetAccountParasModel encryptedData, bool isInternalRequest)
        {
            var criteria = new AccountGetCriteria(isInternalRequest)
            {
                JsonAccountInfo = JsonConvert.SerializeObject(encryptedData)
            };

            return repository.Fetch<GetAccountResultModel>(criteria);
        }

        public IEnumerable<ParticipantModel> GetParticipant(bool isInternalRequest)
        {
            var criteria = new ParticipantGetCriteria(isInternalRequest);

            return repository.Fetch<ParticipantModel>(criteria);
        }

        public AccountDBResult AddAccounts(int? participantID, IEnumerable<InsertAccountParasModel> encryptedData, bool isInternalRequest)
        {
            var criteria = new AccountInsertCriteria(isInternalRequest)
            {
                JsonAccountInfo = JsonConvert.SerializeObject(encryptedData),
                ParticipantID = participantID
            };

            repository.Execute(criteria);

            return new AccountDBResult
            {
                StatusCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? HttpStatus.Created : HttpStatus.BadRequest,
                ErrorMessage = criteria.ErrorMessage
            };
        }

        public AccountDBResult UpdateAccounts(int? participantID, IEnumerable<InsertAccountParasModel> encryptedData, bool isInternalRequest)
        {
            var criteria = new AccountUpdateCriteria(isInternalRequest)
            {
                JsonAccountInfo = JsonConvert.SerializeObject(encryptedData),
                ParticipantID = participantID
            };

            repository.Execute(criteria);

            return new AccountDBResult
            {
                StatusCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? HttpStatus.OK : HttpStatus.BadRequest,
                ErrorMessage = criteria.ErrorMessage
            };
        }

        public AccountDBResult DeleteAccounts(int? participantID, IEnumerable<DeleteAccountParasModel> accountIDs, bool isInternalRequest)
        {
            var criteria = new AccountDeleteCriteria(isInternalRequest)
            {
                JsonAccountInfo = JsonConvert.SerializeObject(accountIDs),
                ParticipantID = participantID
            };

            repository.Execute(criteria);

            return new AccountDBResult { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty };
        }

        public IEnumerable<CheckAccountModel> CheckAccountValidation(int? participantID, IEnumerable<DeleteAccountParasModel> accountIDs, bool isInternalRequest)
        {
            var criteria = new AccountValidationCheckCriteria(isInternalRequest)
            {
                JsonAccountInfo = JsonConvert.SerializeObject(accountIDs),
                ParticipantID = participantID
            };

            var result = repository.Fetch<CheckAccountModel>(criteria);

            return result;
        }
    }
}