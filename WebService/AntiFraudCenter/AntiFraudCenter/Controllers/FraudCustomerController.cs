namespace AntiFraudCenter.Controllers
{
    using AntiFraudCenterService.Features.FraudCustomer.Model;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;
    using static AntiFraudCenterService.Shares.Constants;

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
            if (string.IsNullOrEmpty(paras.EncryptedData) || paras.ParticipantID == null)
            {
                return BadRequest("Invalid input data");
            }

            var result = this.fraudCustomerService.GetFraudCustomer(paras.ParticipantID, paras.EncryptedData);

            if (result.ErrorCode == HttpStatus.OK)
            {
                return Ok(result.FraudCusts);
            }

            return result.ErrorCode == HttpStatus.BadRequest ? BadRequest(result.ErrorMsg) : NotFound();
        }

        [HttpGet("GetFraudCustomerDescryption")]
        public IActionResult GetFraudCustomerDecryption(int participantID)
        {
            return Ok(this.fraudCustomerService.GetFraudCustomerDecryption(participantID));
        }

        [HttpPost("AddFraudCustomer")]
        public IActionResult AddFraudCustomer([FromBody] FraudCustomerParasModel paras)
        {
            if (string.IsNullOrEmpty(paras.EncryptedData) || paras.ParticipantID == null)
            {
                return BadRequest("Invalid input data");
            }

            var result = this.fraudCustomerService.AddFraudCustomer(paras.ParticipantID, paras.EncryptedData);

            if (result.ErrorCode == HttpStatus.OK)
            {
                return Ok(true);
            }

            return result.ErrorCode == HttpStatus.BadRequest ? BadRequest(result.ErrorMsg) : StatusCode((int)result.ErrorCode, result.ErrorMsg);
        }

        [HttpPut("UpdateFraudCustomer")]
        public IActionResult UpdateFraudCustomer([FromBody] FraudCustomerParasModel paras)
        {
            if (string.IsNullOrEmpty(paras.EncryptedData) || paras.ParticipantID == null)
            {
                return BadRequest("Invalid input data");
            }

            var result = this.fraudCustomerService.UpdateFraudCustomer(paras.ParticipantID, paras.EncryptedData);

            if (result.ErrorCode == HttpStatus.OK)
            {
                return Ok(true);
            }

            return result.ErrorCode == HttpStatus.BadRequest ? BadRequest(result.ErrorMsg) : StatusCode((int)result.ErrorCode, result.ErrorMsg);
        }

        [HttpDelete("DeleteFraudCustomer")]
        public IActionResult DeleteFraudCustomer([FromBody] FraudCustomerParasModel paras)
        {
            if (string.IsNullOrEmpty(paras.EncryptedData) || paras.ParticipantID == null)
            {
                return BadRequest("Invalid input data");
            }

            var result = this.fraudCustomerService.DeleteFraudCustomer(paras.ParticipantID, paras.EncryptedData);

            if (result.ErrorCode == HttpStatus.OK)
            {
                return Ok(true);
            }

            return result.ErrorCode == HttpStatus.BadRequest ? BadRequest(result.ErrorMsg) : StatusCode((int)result.ErrorCode, result.ErrorMsg);
        }
    }
}