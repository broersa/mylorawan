using System;
using log4net;

namespace Com.Bekijkhet.Logger
{
    public class Log
    {
        public static void Info(ILog log, string message, DateTime duration)
        {
            log.Info(message);
        }

        public static void Error(ILog log, string message, DateTime duration, Exception ex)
        {
            log.Error(message, ex);
        }
    }
}

