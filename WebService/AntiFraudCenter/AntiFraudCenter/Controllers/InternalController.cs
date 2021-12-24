namespace AntiFraudCenter.Controllers
{
    using AntiFraudCenterService.Features.Participants.Model;
    using AntiFraudCenterService.Shares;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;
    using static AntiFraudCenterService.Shares.Constants;

    [Route("api/Internal")]
    [ApiController]
    public class InternalController : ControllerBase
    {
        private readonly ISentryClient sentryClient;
        private readonly IParticipantService participantService;
        private readonly IAppSettings appSettings;

        public InternalController(ISentryClient sentryClient,
            IParticipantService participantService,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.participantService = participantService;
            this.appSettings = appSettings;
        }

        //Not expose API for participants
        [HttpPost("InsertParticipant")]
        public IActionResult InsertParticipant([FromBody] CreateParticipantParaModel para)
        {
            if (UMHelper.IsUnderMaintaince(appSettings.IsUnderMaintenanceTime, appSettings.UMFrom, appSettings.UMTo))
            {
                return StatusCode((int)HttpStatus.InternalServerError, new ConventionViewModel { StatusCode = HttpStatus.InternalServerError, ErrorMessage = UMMessage + $" From {appSettings.UMFrom} - To {appSettings.UMTo} UTC" });
            }

            var result = this.participantService.InitParticipant(para.ParticipantName, para.Password, para.IPs);

            return StatusCode((int)result.StatusCode, result);
        }
    }
}