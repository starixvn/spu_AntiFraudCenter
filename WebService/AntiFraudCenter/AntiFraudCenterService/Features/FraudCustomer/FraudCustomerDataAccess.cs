namespace AntiFraudCenterService.Features.FraudCustomer
{
    using AntiFraudCenterService.Features.FraudCustomer.Criteria;
    using Fanex.Data.Repository;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public interface IFraudCustomerDataAccess
    {
        IEnumerable<GetFraudCustomerModel> GetFraudCustomers(string decryptData);

        IEnumerable<ParticipantModel> GetParticipant();
    }

    public class FraudCustomerDataAccess : IFraudCustomerDataAccess
    {
        private readonly IDynamicRepository repository;

        public FraudCustomerDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<GetFraudCustomerModel> GetFraudCustomers(string decryptData)
        {
            var criteria = new FraudCustomerGetCriteria
            {
                JsonAccountInfo = JsonConvert.SerializeObject(decryptData)
            };

            return repository.Fetch<GetFraudCustomerModel>(criteria);
        }

        public IEnumerable<ParticipantModel> GetParticipant()
        {
            var criteria = new ParticipantGetCriteria();

            return repository.Fetch<ParticipantModel>(criteria);
        }
    }
}