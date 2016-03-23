using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text;
using Com.Bekijkhet.Semtech;
using Microsoft.ApplicationInsights.Extensibility;
using Com.Bekijkhet.Logger;
using TinyIoC;
using Com.Bekijkhet.MyRouter.BrokerClient;
using Com.Bekijkhet.Lora;
using Com.Bekijkhet.MyRouter.Dal;

namespace Com.Bekijkhet.MyRouter.Console
{
    class Program
    {
        public static void Main (string[] args)
        {
            var now = DateTime.UtcNow;
            var container = TinyIoCContainer.Current;
            container.Register<ISemtech, SemtechImpl> ().AsMultiInstance();
            container.Register<ILora, LoraImpl>().AsMultiInstance();
            var dalconfig = new Com.Bekijkhet.MyRouter.DalPsql.DalConfig(Environment.GetEnvironmentVariable("MYROUTER_DB"));
            container.Register<Com.Bekijkhet.MyRouter.DalPsql.DalConfig>(dalconfig);
            container.Register<IDal, Com.Bekijkhet.MyRouter.DalPsql.Dal>().AsMultiInstance();
            container.Register<IBrokerClient, Com.Bekijkhet.MyRouter.BrokerClientImpl.BrokerClient>().AsMultiInstance();
            container.Register<IProcessor, ProcessorImpl> ().AsMultiInstance();
            //TelemetryConfiguration.Active.InstrumentationKey = Environment.GetEnvironmentVariable("MYROUTER_APPINSIGHT");
            //Log.Info ("Starting MyRouter...", now);
            UDPListener ().Wait();
        }

        private static async Task UDPListener()
        {
            var now = DateTime.UtcNow;
            var container = TinyIoCContainer.Current;
            using (var udpClient = new UdpClient(1700))
            {
                while (true)
                {
                    try {
                        var processor = container.Resolve<IProcessor>();
                        processor.Process(await udpClient.ReceiveAsync(), udpClient);
                    }
                    catch (Exception e)
                    {
                        //Log.Error(e, now);
                    }
                }
            }
        }
    }
}
