namespace AntiFraudCenter.Controllers
{
    using AntiFraudCenterService.Features.FraudCustomer;
    using AntiFraudCenterService.Features.FraudCustomer.Model;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;

    [ApiController]
    [Route("api/Account")]
    public class AccountController : ControllerBase
    {
        private readonly ISentryClient sentryClient;
        private readonly IAccountService accountService;

        public AccountController(ISentryClient sentryClient,
            IAccountService accountService)
        {
            this.sentryClient = sentryClient;
            this.accountService = accountService;
        }

        [HttpPost("Get")]
        public IActionResult GetAccounts([FromBody] GetAccountParasViewModel paras)
        {
            var result = this.accountService.GetAccounts(paras.Key, paras.Data);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("Insert")]
        public IActionResult InsertAccounts([FromBody] InsertAccountParasViewModel paras)
        {
            var result = this.accountService.InsertAccounts(paras.Key, paras.Data);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("Update")]
        public IActionResult UpdateAccounts([FromBody] InsertAccountParasViewModel paras)
        {
            var result = this.accountService.UpdateAccounts(paras.Key, paras.Data);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("Delete")]
        public IActionResult DeleteAccounts([FromBody] DeleteAccountParasViewModel paras)
        {
            var result = this.accountService.DeleteAccounts(paras.Key, paras.Data);

            return StatusCode((int)result.StatusCode, result);
        }
    }
}