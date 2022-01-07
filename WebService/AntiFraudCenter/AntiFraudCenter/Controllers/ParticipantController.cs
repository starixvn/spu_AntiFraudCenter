namespace AntiFraudCenter.Controllers
{
    using AntiFraudCenterService.Features.Participants.Model;
    using AntiFraudCenterService.Shares;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;
    using static AntiFraudCenterService.Shares.Constants;

    [Route("api/Participant")]
    [ApiController]
    public class ParticipantController : ControllerBase
    {
        private readonly ISentryClient sentryClient;
        private readonly IParticipantService participantService;
        private readonly IAppSettings appSettings;

        public ParticipantController(ISentryClient sentryClient,
            IParticipantService participantService,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.participantService = participantService;
            this.appSettings = appSettings;
        }

        [HttpGet("Get")]
        public IActionResult GetParticipantName()
        {
            if (UMHelper.IsUnderMaintaince(appSettings.IsUnderMaintenanceTime, appSettings.UMFrom, appSettings.UMTo))
            {
                return StatusCode((int)HttpStatus.InternalServerError, new GetParticipantNameResultModel { StatusCode = HttpStatus.InternalServerError, ErrorMessage = UMMessage + $" From {appSettings.UMFrom} - To {appSettings.UMTo} UTC", Data = null});
            }

            var result = this.participantService.GetParticipantName();

            return StatusCode((int)result.StatusCode, result);
        }
    }
}