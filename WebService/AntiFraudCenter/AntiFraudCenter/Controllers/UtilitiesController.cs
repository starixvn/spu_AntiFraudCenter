namespace AntiFraudCenter.Controllers
{
    using AntiFraudCenterService.Features.Participants.Model;
    using AntiFraudCenterService.Shares;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;
    using System;
    using static AntiFraudCenterService.Shares.Constants;

    [Route("api/Utilities")]
    [ApiController]
    public class UtilitiesController : ControllerBase
    {
        private readonly ISentryClient sentryClient;
        private readonly IParticipantService participantService;
        private readonly IAppSettings appSettings;

        public UtilitiesController(ISentryClient sentryClient,
            IParticipantService participantService,
             IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.participantService = participantService;
            this.appSettings = appSettings;
        }

        [HttpPost]
        [Route("RegenerateKey")]
        public IActionResult RegenerateKey([FromBody] string key)
        {
            if (UMHelper.IsUnderMaintaince(appSettings.IsUnderMaintenanceTime, appSettings.UMFrom, appSettings.UMTo))
            {
                return StatusCode((int)HttpStatus.InternalServerError, new RegenerateKeyResultModel { StatusCode = HttpStatus.InternalServerError, ErrorMessage = UMMessage + $" From {appSettings.UMFrom} - To {appSettings.UMTo} UTC" });
            }

            var result = this.participantService.UpdateParticipantSecretKey(key);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("GetConvention")]
        public IActionResult GetConvention()
        {
            if (UMHelper.IsUnderMaintaince(appSettings.IsUnderMaintenanceTime, appSettings.UMFrom, appSettings.UMTo))
            {
                return StatusCode((int)HttpStatus.InternalServerError, new ConventionResultModel { StatusCode = HttpStatus.InternalServerError, ErrorMessage = UMMessage + $" From {appSettings.UMFrom} - To {appSettings.UMTo} UTC" });
            }

            var result = this.participantService.GetConventionInfo();

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("CheckHealth")]
        public IActionResult CheckHealth()
        {
            sentryClient.CaptureMessage("AFC - Check Health OK! ", Sentry.Protocol.SentryLevel.Info);

            return StatusCode((int)HttpStatus.OK, $"{DateTime.UtcNow} UTC");
        }
    }
}