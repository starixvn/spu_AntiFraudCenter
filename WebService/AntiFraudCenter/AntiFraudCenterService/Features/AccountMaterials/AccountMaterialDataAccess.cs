namespace AntiFraudCenterService.Features.AccountMaterials
{
    using AntiFraudCenterService.Features.AccountMaterials.Criteria;
    using AntiFraudCenterService.Shares.Model;
    using Fanex.Data.Repository;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using static AntiFraudCenterService.Shares.Constants;

    public interface IAccountMaterialDataAccess
    {
        AccountDBResult InsertIPs(int? participantID, IEnumerable<InsertIPParasModel> encryptedData, bool isInternalRequest);

        AccountDBResult InsertDevices(int? participantID, IEnumerable<InsertDeviceParasModel> encryptedData, bool isInternalRequest);

        AccountDBResult DeleteIPs(int? participantID, IEnumerable<DeleteIPParasModel> accountIDs, bool isInternalRequest);

        AccountDBResult DeleteDevices(int? participantID, IEnumerable<DeleteDeviceParasModel> accountIDs, bool isInternalRequest);
    }

    public class AccountMaterialDataAccess : IAccountMaterialDataAccess
    {
        private readonly IDynamicRepository repository;

        public AccountMaterialDataAccess()
        {
            repository = new DynamicRepository();
        }

        public AccountDBResult InsertIPs(int? participantID, IEnumerable<InsertIPParasModel> encryptedData, bool isInternalRequest)
        {
            var criteria = new IPInsertCriteria(isInternalRequest)
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

        public AccountDBResult InsertDevices(int? participantID, IEnumerable<InsertDeviceParasModel> encryptedData, bool isInternalRequest)
        {
            var criteria = new DeviceInsertCriteria(isInternalRequest)
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

        public AccountDBResult DeleteIPs(int? participantID, IEnumerable<DeleteIPParasModel> accountIDs, bool isInternalRequest)
        {
            var criteria = new IPDeleteCriteria(isInternalRequest)
            {
                JsonAccountInfo = JsonConvert.SerializeObject(accountIDs),
                ParticipantID = participantID
            };

            repository.Execute(criteria);

            return new AccountDBResult { StatusCode = HttpStatus.OK, ErrorMessage = criteria.ErrorMessage };
        }

        public AccountDBResult DeleteDevices(int? participantID, IEnumerable<DeleteDeviceParasModel> accountIDs, bool isInternalRequest)
        {
            var criteria = new DeviceDeleteCriteria(isInternalRequest)
            {
                JsonAccountInfo = JsonConvert.SerializeObject(accountIDs),
                ParticipantID = participantID
            };

            repository.Execute(criteria);

            return new AccountDBResult { StatusCode = HttpStatus.OK, ErrorMessage = criteria.ErrorMessage };
        }
    }
}