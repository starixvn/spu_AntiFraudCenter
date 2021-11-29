namespace AntiFraudCenter.Controllers
{
    using AntiFraudCenterService.Features.FraudCustomer;
    using AntiFraudCenterService.Features.FraudCustomer.Model;
    using AntiFraudCenterService.Shares;
    using AntiFraudCenterService.Shares.Model;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;
    using static AntiFraudCenterService.Shares.Constants;

    [ApiController]
    [Route("api/Account")]
    public class AccountController : ControllerBase
    {
        private readonly ISentryClient sentryClient;
        private readonly IAccountService accountService;
        private readonly IAppSettings appSettings;

        public AccountController(ISentryClient sentryClient,
            IAccountService accountService,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.accountService = accountService;
            this.appSettings = appSettings;
        }

        [HttpPost("Get")]
        public IActionResult GetAccounts([FromBody] GetAccountParasViewModel paras)
        {
            if (UMHelper.IsUnderMaintaince(appSettings.IsUnderMaintenanceTime, appSettings.UMFrom, appSettings.UMTo))
            {
                return StatusCode((int)HttpStatus.InternalServerError, new AccountDBResult { StatusCode = HttpStatus.InternalServerError, ErrorMessage = UMMessage + $" From {appSettings.UMFrom} - To {appSettings.UMTo} UTC" });
            }

            var result = this.accountService.GetAccounts(paras.Key, paras.Data);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("Insert")]
        public IActionResult InsertAccounts([FromBody] InsertAccountParasViewModel paras)
        {
            if (UMHelper.IsUnderMaintaince(appSettings.IsUnderMaintenanceTime, appSettings.UMFrom, appSettings.UMTo))
            {
                return StatusCode((int)HttpStatus.InternalServerError, new AccountDBResult { StatusCode = HttpStatus.InternalServerError, ErrorMessage = UMMessage + $" From {appSettings.UMFrom} - To {appSettings.UMTo} UTC" });
            }

            var result = this.accountService.InsertAccounts(paras.Key, paras.Data);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("Update")]
        public IActionResult UpdateAccounts([FromBody] InsertAccountParasViewModel paras)
        {
            if (UMHelper.IsUnderMaintaince(appSettings.IsUnderMaintenanceTime, appSettings.UMFrom, appSettings.UMTo))
            {
                return StatusCode((int)HttpStatus.InternalServerError, new AccountDBResult { StatusCode = HttpStatus.InternalServerError, ErrorMessage = UMMessage + $" From {appSettings.UMFrom} - To {appSettings.UMTo} UTC" });
            }

            var result = this.accountService.UpdateAccounts(paras.Key, paras.Data);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("Delete")]
        public IActionResult DeleteAccounts([FromBody] DeleteAccountParasViewModel paras)
        {
            if (UMHelper.IsUnderMaintaince(appSettings.IsUnderMaintenanceTime, appSettings.UMFrom, appSettings.UMTo))
            {
                return StatusCode((int)HttpStatus.InternalServerError, new AccountDBResult { StatusCode = HttpStatus.InternalServerError, ErrorMessage = UMMessage + $" From {appSettings.UMFrom} - To {appSettings.UMTo} UTC" });
            }

            var result = this.accountService.DeleteAccounts(paras.Key, paras.Data);

            return StatusCode((int)result.StatusCode, result);
        }
    }
}