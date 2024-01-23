namespace AntiFraudCenterService.Features.Authorization
{
    using AntiFraudCenterService.Features.Authorization.Criteria;
    using Fanex.Data.Repository;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public interface IAuthorizationDataAccess
    {
        IEnumerable<CheckAccountModel> CheckAccountValidation(int? participantID, IEnumerable<AccountsModel> accountIDs, bool isInternalRequest);
    }

    public class AuthorizationDataAccess : IAuthorizationDataAccess
    {
        private readonly IDynamicRepository repository;

        public AuthorizationDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<CheckAccountModel> CheckAccountValidation(int? participantID, IEnumerable<AccountsModel> accountIDs, bool isInternalRequest)
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