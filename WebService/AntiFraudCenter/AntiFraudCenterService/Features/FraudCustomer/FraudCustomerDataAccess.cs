namespace AntiFraudCenterService.Features.FraudCustomer
{
    using AntiFraudCenterService.Features.FraudCustomer.Criteria;
    using AntiFraudCenterService.Shares.Model;
    using Fanex.Data.Repository;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using static AntiFraudCenterService.Shares.Constants;

    public interface IFraudCustomerDataAccess
    {
        IEnumerable<GetFraudCustomerModel> GetFraudCustomers(string decryptData);

        IEnumerable<ParticipantModel> GetParticipant();

        DBResult AddFraudCustomers(int? participantID, string decryptData);

        DBResult UpdateFraudCustomers(int? participantID, string decryptData);

        DBResult DeleteFraudCustomers(int? participantID, string decryptData);
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

        public DBResult AddFraudCustomers(int? participantID, string decryptData)
        {
            var criteria = new FraudCustomerAddCriteria
            {
                JsonAccountInfo = decryptData,
                ParticipantID = participantID
            };

            repository.Execute(criteria);

            return new DBResult
            {
                ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? HttpStatus.OK : HttpStatus.ServerNotImplemented,
                ErrorMsg = criteria.ErrorMessage
            };
        }

        public DBResult UpdateFraudCustomers(int? participantID, string decryptData)
        {
            var criteria = new FraudCustomerUpdateCriteria
            {
                JsonAccountInfo = decryptData,
                ParticipantID = participantID
            };

            repository.Execute(criteria);

            return new DBResult
            {
                ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? HttpStatus.OK : HttpStatus.ServerNotImplemented,
                ErrorMsg = criteria.ErrorMessage
            };
        }

        public DBResult DeleteFraudCustomers(int? participantID, string decryptData)
        {
            var criteria = new FraudCustomerDeleteCriteria
            {
                JsonAccountInfo = decryptData,
                ParticipantID = participantID
            };

            repository.Execute(criteria);

            return new DBResult
            {
                ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? HttpStatus.OK : HttpStatus.ServerNotImplemented,
                ErrorMsg = criteria.ErrorMessage
            };
        }
    }
}