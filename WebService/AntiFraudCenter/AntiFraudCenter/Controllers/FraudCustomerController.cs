namespace AntiFraudCenter.Controllers
{
    using AntiFraudCenterService.Features.FraudCustomer;
    using AntiFraudCenterService.Features.FraudCustomer.Model;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;
    using System.Collections.Generic;
    using System.Linq;
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
                return BadRequest("Data input invalid");
            }

            var result = this.fraudCustomerService.GetFraudCustomer(paras.ParticipantID, paras.EncryptedData);

            if(result.ErrorCode == HttpStatus.OK)
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
    }
}
