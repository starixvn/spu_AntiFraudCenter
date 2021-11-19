namespace AntiFraudCenter.Controllers
{
    using AntiFraudCenterService.Features.ParticipantInformation.Model;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;

    [Route("api/Internal")]
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
        [HttpPost("InsertParticipant")]
        public IActionResult InsertParticipant([FromBody] CreateParticipantParaModel para)
        {
            var result = this.participantService.InitParticipant(para.ParticipantName, para.Password, para.IPs);

            return StatusCode((int)result.StatusCode, result);
        }
    }
}