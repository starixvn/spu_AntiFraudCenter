namespace AntiFraudCenter.Controllers
{
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
        public IActionResult GetFraudCustomer([FromBody] FraudCustomerParasModel paras)
        {
            var result = this.fraudCustomerService.GetFraudCustomer(paras.ParticipantID, paras.EncryptedData);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("GetFraudCustomerDescryption")]
        public IActionResult GetFraudCustomerDecryption(int participantID)
        {
            return Ok(this.fraudCustomerService.GetFraudCustomerDecryption(participantID));
        }

        [HttpPost("AddFraudCustomer")]
        public IActionResult AddFraudCustomer([FromBody] FraudCustomerParasModel paras)
        {
            var result = this.fraudCustomerService.AddFraudCustomer(paras.ParticipantID, paras.EncryptedData);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut("UpdateFraudCustomer")]
        public IActionResult UpdateFraudCustomer([FromBody] FraudCustomerParasModel paras)
        {
            var result = this.fraudCustomerService.UpdateFraudCustomer(paras.ParticipantID, paras.EncryptedData);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("DeleteFraudCustomer")]
        public IActionResult DeleteFraudCustomer([FromBody] FraudCustomerParasModel paras)
        {
            var result = this.fraudCustomerService.DeleteFraudCustomer(paras.ParticipantID, paras.EncryptedData);

            return StatusCode((int)result.StatusCode, result);
        }
    }
}