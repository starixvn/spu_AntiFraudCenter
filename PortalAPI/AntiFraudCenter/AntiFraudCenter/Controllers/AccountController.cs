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
        private readonly IFraudCustomerService fraudCustomerService;

        public AccountController(ISentryClient sentryClient,
            IFraudCustomerService fraudCustomerService)
        {
            this.sentryClient = sentryClient;
            this.fraudCustomerService = fraudCustomerService;
        }

        [HttpPost("Get")]
        public IActionResult GetAccounts([FromBody] GetFraudCustomerParasViewModel paras)
        {
            var result = this.fraudCustomerService.GetAccounts(paras.Key, paras.Data);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("Insert")]
        public IActionResult InsertAccounts([FromBody] InsertFraudCustParasViewModel paras)
        {
            var result = this.fraudCustomerService.AddFraudCustomer(paras.Key, paras.Data);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut("Update")]
        public IActionResult UpdateAccounts([FromBody] InsertFraudCustParasViewModel paras)
        {
            var result = this.fraudCustomerService.UpdateFraudCustomer(paras.Key, paras.Data);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("Delete")]
        public IActionResult DeleteAccounts([FromBody] DeleteFraudCustParasViewModel paras)
        {
            var result = this.fraudCustomerService.DeleteFraudCustomer(paras.Key, paras.Data);

            return StatusCode((int)result.StatusCode, result);
        }
    }
}