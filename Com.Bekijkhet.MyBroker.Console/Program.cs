using System;
using Mono.Unix;
using Nancy.Hosting.Self;
using Mono.Unix.Native;

namespace Com.Bekijkhet.MyBroker.Console
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var uri = "http://localhost:8888";
            System.Console.WriteLine("Starting Nancy on " + uri);

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

            System.Console.WriteLine("Stopping Nancy");
            host.Stop();  // stop hosting
        }
    }
}
