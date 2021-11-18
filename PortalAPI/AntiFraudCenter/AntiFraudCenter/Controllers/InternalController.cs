namespace AntiFraudCenter.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Sentry;

    [Route("api/private")]
    [ApiController]
    public class InternalController : ControllerBase
    {
        private readonly ISentryClient sentryClient;
        private readonly IParticipantService participantService;

        public InternalController(ISentryClient sentryClient,
            IParticipantService participantService)
        {
            this.sentryClient = sentryClient;
            this.participantService = participantService;
        }

        //Not expose API for participants
        [HttpPost("InitParticipant")]
        public IActionResult InitParticipant([FromForm] string password, [FromForm] string participantName)
        {
            var result = this.participantService.InitParticipant(participantName, password);

            return StatusCode((int)result.StatusCode, result);
        }
    }
}