namespace AntiFraudCenter.Controllers
{
    using AntiFraudCenterService.Features.ParticipantInformation.Model;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;

  
    [Route("api/participant")]
    [ApiController]
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

        [HttpPut("ReGenerateSecretKey")]
        public IActionResult ReGenerateSecretKey([FromBody] GenerateKeyParasModel paras)
        {
            var result = this.participantService.UpdateParticipantSecretKey(paras.ParticipantID, paras.EncryptedSecretKey);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("GetConvention")]
        public IActionResult GetConvention()
        {
            var result = this.participantService.GetConventionInfo();

            return StatusCode((int)result.StatusCode, result);
        }
    }
}