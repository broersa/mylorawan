using System;
using Microsoft.ApplicationInsights;

namespace Com.Bekijkhet.Logger
{
    public static class Log
    {
        private static TelemetryClient _telemetry = new TelemetryClient();

        public static void Info(string message, DateTime duration)
        {
            _telemetry.TrackTrace(message, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Information);
            _telemetry.Flush ();
        }

        public static void Error(Exception e, DateTime duration)
        {
            _telemetry.TrackException(e);
            _telemetry.Flush ();
        }
    }
}

