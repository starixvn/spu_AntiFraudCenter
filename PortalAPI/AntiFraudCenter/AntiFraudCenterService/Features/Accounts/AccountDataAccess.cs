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
        IEnumerable<GetAccountResultModel> GetAccounts(GetAccountParasModel encryptedData);

        IEnumerable<ParticipantModel> GetParticipant();

        AccountDBResult AddAccounts(int? participantID, IEnumerable<InsertAccountParasModel> encryptedData);

        AccountDBResult UpdateAccounts(int? participantID, IEnumerable<InsertAccountParasModel> encryptedData);

        AccountDBResult DeleteAccounts(int? participantID, IEnumerable<DeleteAccountParasModel> accountIDs);
    }

    public class AccountDataAccess : IAccountDataAccess
    {
        private readonly IDynamicRepository repository;

        public AccountDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<GetAccountResultModel> GetAccounts(GetAccountParasModel encryptedData)
        {
            var criteria = new AccountGetCriteria
            {
                JsonAccountInfo = JsonConvert.SerializeObject(encryptedData)
            };

            return repository.Fetch<GetAccountResultModel>(criteria);
        }

        public IEnumerable<ParticipantModel> GetParticipant()
        {
            var criteria = new ParticipantGetCriteria();

            return repository.Fetch<ParticipantModel>(criteria);
        }

        public AccountDBResult AddAccounts(int? participantID, IEnumerable<InsertAccountParasModel> encryptedData)
        {
            var criteria = new AccountInsertCriteria
            {
                JsonAccountInfo = JsonConvert.SerializeObject(encryptedData),
                ParticipantID = participantID
            };

            repository.Execute(criteria);

            return new AccountDBResult
            {
                StatusCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? HttpStatus.Created : HttpStatus.InternalServerError,
                ErrorMessage = criteria.ErrorMessage
            };
        }

        public AccountDBResult UpdateAccounts(int? participantID, IEnumerable<InsertAccountParasModel> encryptedData)
        {
            var criteria = new AccountUpdateCriteria
            {
                JsonAccountInfo = JsonConvert.SerializeObject(encryptedData),
                ParticipantID = participantID
            };

            repository.Execute(criteria);

            return new AccountDBResult
            {
                StatusCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? HttpStatus.OK : HttpStatus.InternalServerError,
                ErrorMessage = criteria.ErrorMessage
            };
        }

        public AccountDBResult DeleteAccounts(int? participantID, IEnumerable<DeleteAccountParasModel> accountIDs)
        {
            var criteria = new AccountDeleteCriteria
            {
                JsonAccountInfo = JsonConvert.SerializeObject(accountIDs),
                ParticipantID = participantID
            };

            repository.Execute(criteria);

            return new AccountDBResult { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty };
        }
    }
}