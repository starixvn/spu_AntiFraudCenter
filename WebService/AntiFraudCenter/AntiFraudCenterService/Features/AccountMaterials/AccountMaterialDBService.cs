namespace AntiFraudCenterService.Features.AccountMaterials
{
    using AntiFraudCenterService.Features.Accounts;
    using AntiFraudCenterService.Shares;
    using AntiFraudCenterService.Shares.Model;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using static AntiFraudCenterService.Shares.Constants;

    public interface IAccountMaterialDBService
    {
        AccountDBResult InsertIPs(IEnumerable<InsertIPParasModel> data, int? participantID, bool isInternalRequest, int batchSize);

        AccountDBResult InsertDevices(IEnumerable<InsertDeviceParasModel> data, int? participantID, bool isInternalRequest, int batchSize);

        AccountDBResult DeleteIPs(IEnumerable<DeleteIPParasModel> data, int? participantID, bool isInternalRequest, int batchSize);

        AccountDBResult DeleteDevices(IEnumerable<DeleteDeviceParasModel> data, int? participantID, bool isInternalRequest, int batchSize);
    }

    public class AccountMaterialDBService : IAccountMaterialDBService
    {
        private readonly IAccountMaterialDataAccess accountMaterialDataAccess;
        private readonly IAccountDBService accountDBService;

        public AccountMaterialDBService(
                IAccountMaterialDataAccess accountMaterialDataAccess,
                IAccountDBService accountDBService)
        {
            this.accountMaterialDataAccess = accountMaterialDataAccess;
            this.accountDBService = accountDBService;
        }

        public AccountDBResult InsertIPs(IEnumerable<InsertIPParasModel> data, int? participantID, bool isInternalRequest, int batchSize)
        {
            if (data != null && data.Any() && participantID != null)
            {
                var dataChunk = data.ChunkBy(batchSize);

                foreach (var dataBatch in dataChunk)
                {
                    var result = accountMaterialDataAccess.InsertIPs(participantID, dataBatch, isInternalRequest);

                    if (!string.IsNullOrEmpty(result.ErrorMessage))
                    {
                        accountDBService.LogErrorToDB(participantID, result.ErrorMessage, JsonConvert.SerializeObject(data), isInternalRequest);
                    }
                }

                return new AccountDBResult() { StatusCode = HttpStatus.Created, ErrorMessage = string.Empty };
            }

            return new AccountDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
        }

        public AccountDBResult InsertDevices(IEnumerable<InsertDeviceParasModel> data, int? participantID, bool isInternalRequest, int batchSize)
        {
            if (data != null && data.Any() && participantID != null)
            {
                var dataChunk = data.ChunkBy(batchSize);

                foreach (var dataBatch in dataChunk)
                {
                    var result = accountMaterialDataAccess.InsertDevices(participantID, dataBatch, isInternalRequest);

                    if (!string.IsNullOrEmpty(result.ErrorMessage))
                    {
                        accountDBService.LogErrorToDB(participantID, result.ErrorMessage, JsonConvert.SerializeObject(data), isInternalRequest);
                    }
                }

                return new AccountDBResult() { StatusCode = HttpStatus.Created, ErrorMessage = string.Empty };
            }

            return new AccountDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
        }

        public AccountDBResult DeleteIPs(IEnumerable<DeleteIPParasModel> data, int? participantID, bool isInternalRequest, int batchSize)
        {
            if (data != null && data.Any() && participantID != null)
            {
                var dataChunk = data.ChunkBy(batchSize);

                foreach (var dataBatch in dataChunk)
                {
                    var result = accountMaterialDataAccess.DeleteIPs(participantID, dataBatch, isInternalRequest);

                    if (!string.IsNullOrEmpty(result.ErrorMessage))
                    {
                        accountDBService.LogErrorToDB(participantID, result.ErrorMessage, JsonConvert.SerializeObject(data), isInternalRequest);
                    }
                }

                return new AccountDBResult() { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty };
            }
            return new AccountDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
        }

        public AccountDBResult DeleteDevices(IEnumerable<DeleteDeviceParasModel> data, int? participantID, bool isInternalRequest, int batchSize)
        {
            if (data != null && data.Any() && participantID != null)
            {
                var dataChunk = data.ChunkBy(batchSize);

                foreach (var dataBatch in dataChunk)
                {
                    var result = accountMaterialDataAccess.DeleteDevices(participantID, dataBatch, isInternalRequest);

                    if (!string.IsNullOrEmpty(result.ErrorMessage))
                    {
                        accountDBService.LogErrorToDB(participantID, result.ErrorMessage, JsonConvert.SerializeObject(data), isInternalRequest);
                    }
                }

                return new AccountDBResult() { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty };
            }
            return new AccountDBResult() { StatusCode = HttpStatus.BadRequest, ErrorMessage = InvalidInput };
        }
    }
}