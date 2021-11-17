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
        IEnumerable<GetFraudCustResultModel> GetFraudCustomers(GetFraudCustParasModel encryptedData);

        IEnumerable<ParticipantModel> GetParticipant();

        FraudCustDBResult AddFraudCustomers(int? participantID, IEnumerable<InsertFraudCustParasModel> encryptedData);

        FraudCustDBResult UpdateFraudCustomers(int? participantID, IEnumerable<InsertFraudCustParasModel> encryptedData);

        FraudCustDBResult DeleteFraudCustomers(int? participantID, IEnumerable<DeleteFraudCustParasModel> accountIDs);
    }

    public class FraudCustomerDataAccess : IFraudCustomerDataAccess
    {
        private readonly IDynamicRepository repository;

        public FraudCustomerDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<GetFraudCustResultModel> GetFraudCustomers(GetFraudCustParasModel encryptedData)
        {
            var criteria = new FraudCustomerGetCriteria
            {
                JsonAccountInfo = JsonConvert.SerializeObject(encryptedData)
            };

            return repository.Fetch<GetFraudCustResultModel>(criteria);
        }

        public IEnumerable<ParticipantModel> GetParticipant()
        {
            var criteria = new ParticipantGetCriteria();

            return repository.Fetch<ParticipantModel>(criteria);
        }

        public FraudCustDBResult AddFraudCustomers(int? participantID, IEnumerable<InsertFraudCustParasModel> encryptedData)
        {
            var criteria = new FraudCustomerAddCriteria
            {
                JsonAccountInfo = JsonConvert.SerializeObject(encryptedData),
                ParticipantID = participantID
            };

            repository.Execute(criteria);

            return new FraudCustDBResult
            {
                StatusCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? HttpStatus.OK : HttpStatus.BadRequest,
                ErrorMessage = criteria.ErrorMessage
            };
        }

        public FraudCustDBResult UpdateFraudCustomers(int? participantID, IEnumerable<InsertFraudCustParasModel> encryptedData)
        {
            var criteria = new FraudCustomerUpdateCriteria
            {
                JsonAccountInfo = JsonConvert.SerializeObject(encryptedData),
                ParticipantID = participantID
            };

            repository.Execute(criteria);

            return new FraudCustDBResult
            {
                StatusCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? HttpStatus.OK : HttpStatus.BadRequest,
                ErrorMessage = criteria.ErrorMessage
            };
        }

        public FraudCustDBResult DeleteFraudCustomers(int? participantID, IEnumerable<DeleteFraudCustParasModel> accountIDs)
        {
            var criteria = new FraudCustomerDeleteCriteria
            {
                JsonAccountInfo = JsonConvert.SerializeObject(accountIDs),
                ParticipantID = participantID
            };

            repository.Execute(criteria);

            return new FraudCustDBResult { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty };
        }
    }
}