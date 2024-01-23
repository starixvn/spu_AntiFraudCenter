namespace AntiFraudCenterService.Features.Authorization
{
    using AntiFraudCenterService.Shares.Model;
    using System.Collections.Generic;
    using System.Linq;
    using static AntiFraudCenterService.Shares.Constants;

    public interface IAuthorizationDBService
    {
        AccountDBResult CheckAccountPermisson(IEnumerable<AccountsModel> data, int? participantID, bool isInternalRequest);
    }

    public class AuthorizationDBService : IAuthorizationDBService
    {
        private readonly IAuthorizationDataAccess authorizationDataAccess;

        public AuthorizationDBService(IAuthorizationDataAccess authorizationDataAccess)
        {
            this.authorizationDataAccess = authorizationDataAccess;
        }

        public AccountDBResult CheckAccountPermisson(IEnumerable<AccountsModel> data, int? participantID, bool isInternalRequest)
        {
            var result = new AccountDBResult { StatusCode = HttpStatus.OK, ErrorMessage = string.Empty };

            if (data != null && data.Any())
            {
                result = IsAccountValid(data, participantID, isInternalRequest);
            }

            return result;
        }

        #region Private methods

        private AccountDBResult IsAccountValid(IEnumerable<AccountsModel> data, int? participantID, bool isInternalRequest)
        {
            if (data != null && data.Any())
            {
                var result = authorizationDataAccess.CheckAccountValidation(participantID, data, isInternalRequest);

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

        #endregion Private methods
    }
}