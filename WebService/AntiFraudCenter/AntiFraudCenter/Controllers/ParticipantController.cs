namespace AntiFraudCenter.Controllers
{
    using AntiFraudCenterService.Features.ParticipantInformation.Model;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;
    using static AntiFraudCenterService.Shares.Constants;

    [ApiController]
    [Route("api/participant")]
    public class ParticipantController : ControllerBase
    {
        private readonly ISentryClient sentryClient;
        private readonly IParticipantService participantService;

        public ParticipantController(ISentryClient sentryClient,
            IParticipantService participantService)
        {
            this.sentryClient = sentryClient;
            this.participantService = participantService;
        }

        [HttpPut("RegenerateKey")]
        public IActionResult ReGenerateSecretKey([FromBody] GenerateKeyParasModel paras)
        {
            if (string.IsNullOrEmpty(paras.EncryptedSecretKey) || paras.ParticipantID == null)
            {
                return BadRequest("Invalid input data");
            }

            var result = this.participantService.UpdateParticipantSecretKey(paras.ParticipantID, paras.EncryptedSecretKey);

            if (result.ErrorCode == HttpStatus.OK)
            {
                return Ok(true);
            }

            return result.ErrorCode == HttpStatus.BadRequest ? BadRequest(result.ErrorMsg) : StatusCode((int)result.ErrorCode, result.ErrorMsg);
        }

        [HttpGet("GetConvention")]
        public IActionResult GetConvention()
        {
            return Ok(this.participantService.GetConventionInfo());
        }
    }
}