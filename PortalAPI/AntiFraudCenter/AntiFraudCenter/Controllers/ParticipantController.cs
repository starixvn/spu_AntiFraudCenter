namespace AntiFraudCenter.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Sentry;

    [Route("api/Utilities")]
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

        [HttpPost]
        [Route("RegenerateKey")]
        public IActionResult RegenerateKey([FromBody] string key)
        {
            var result = this.participantService.UpdateParticipantSecretKey(key);

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