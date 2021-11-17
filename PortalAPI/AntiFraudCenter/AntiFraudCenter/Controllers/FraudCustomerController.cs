namespace AntiFraudCenter.Controllers
{
    using AntiFraudCenterService.Features.FraudCustomer;
    using AntiFraudCenterService.Features.FraudCustomer.Model;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;

    [ApiController]
    [Route("api/fraudcase")]
    public class FraudCustomerController : ControllerBase
    {
        private readonly ISentryClient sentryClient;
        private readonly IFraudCustomerService fraudCustomerService;

        public FraudCustomerController(ISentryClient sentryClient,
            IFraudCustomerService fraudCustomerService)
        {
            this.sentryClient = sentryClient;
            this.fraudCustomerService = fraudCustomerService;
        }

        [HttpPost("GetFraudCustomer")]
        public IActionResult GetFraudCustomer([FromBody] GetFraudCustomerParasViewModel paras)
        {
            var result = this.fraudCustomerService.GetFraudCustomer(paras.Key, paras.Data);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("AddFraudCustomer")]
        public IActionResult AddFraudCustomer([FromBody] InsertFraudCustParasViewModel paras)
        {
            var result = this.fraudCustomerService.AddFraudCustomer(paras.Key, paras.Data);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut("UpdateFraudCustomer")]
        public IActionResult UpdateFraudCustomer([FromBody] InsertFraudCustParasViewModel paras)
        {
            var result = this.fraudCustomerService.UpdateFraudCustomer(paras.Key, paras.Data);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("DeleteFraudCustomer")]
        public IActionResult DeleteFraudCustomer([FromBody] DeleteFraudCustParasViewModel paras)
        {
            var result = this.fraudCustomerService.DeleteFraudCustomer(paras.Key, paras.Data);

            return StatusCode((int)result.StatusCode, result);
        }
    }
}