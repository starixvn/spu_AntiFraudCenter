namespace AntiFraudCenterService.Shares
{
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static AntiFraudCenterService.Shares.Constants;

    public static class Utilities
    {
        public static IEnumerable<IEnumerable<T>> ChunkBy<T>(this IEnumerable<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        public static void LogSentryError<T>(string function, string message, T data, Exception ex, ISentryClient sentryClient)
        {
            var logMessage = new LogMessage(ex.Message)
                .AddMessage($"{function}")
                .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                .AddMessage($"{message} - Data {JsonConvert.SerializeObject(data)} - { DateTime.Now}").ToString();

            sentryClient.CaptureMessage(logMessage, Sentry.Protocol.SentryLevel.Error);
        }

        public static void LogSentryError(string function, string message, Exception ex, ISentryClient sentryClient)
        {
            var logMessage = new LogMessage(ex.Message)
                .AddMessage($"{function}")
                .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                .AddMessage($"{message} - { DateTime.Now}").ToString();

            sentryClient.CaptureMessage(logMessage, Sentry.Protocol.SentryLevel.Error);
        }

        public static void LogMonitorRequest(bool isInternalRequest, RequestAction requestAction, string participantName, int? recordNumber, ISentryClient sentryClient)
        {
            if (isInternalRequest)
            {
                return;
            }

            var message = $"Monitor - {Enum.GetName(typeof(RequestAction), requestAction)} - {participantName} - Counter:{recordNumber} - DateTime:{DateTime.UtcNow}";

            sentryClient.CaptureMessage(message, Sentry.Protocol.SentryLevel.Info);
        }
    }
}