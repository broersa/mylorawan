using System;
using Mono.Unix;
using Nancy.Hosting.Self;
using Mono.Unix.Native;
using log4net;
using Com.Bekijkhet.Logger;
using log4net.Config;

namespace Com.Bekijkhet.MyBroker.Console
{
    class MainClass
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Main(string[] args)
        {
            BasicConfigurator.Configure();

            var now = DateTime.UtcNow;

            var uri = "http://localhost:8888";
            Log.Info(log, "Starting MyRouter on " + uri, DateTime.UtcNow);

            // initialize an instance of NancyHost
            var host = new NancyHost(new Uri(uri));
            host.Start();  // start hosting

            // check if we're running on mono
            if (Type.GetType("Mono.Runtime") != null)
            {
                // on mono, processes will usually run as daemons - this allows you to listen
                // for termination signals (ctrl+c, shutdown, etc) and finalize correctly
                UnixSignal.WaitAny(new[] {
                    new UnixSignal(Signum.SIGINT),
                    new UnixSignal(Signum.SIGTERM),
                    new UnixSignal(Signum.SIGQUIT),
                    new UnixSignal(Signum.SIGHUP)
                });
            }
            else
            {
                System.Console.ReadLine();
            }

            Log.Info(log, "Stopping Nancy", DateTime.UtcNow);
            host.Stop();  // stop hosting
        }
    }
}
