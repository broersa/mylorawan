using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text;
using Com.Bekijkhet.Semtech;
using Com.Bekijkhet.Logger;
using TinyIoC;
using Com.Bekijkhet.MyRouter.BrokerClient;
using Com.Bekijkhet.Lora;
using Com.Bekijkhet.MyRouter.Dal;
using log4net;
using log4net.Config;

namespace Com.Bekijkhet.MyRouter.Console
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Main (string[] args)
        {
            BasicConfigurator.Configure();
            var now = DateTime.UtcNow;
            var container = TinyIoCContainer.Current;
            container.Register<ISemtech, SemtechImpl> ().AsMultiInstance();
            container.Register<ILora, LoraImpl>().AsMultiInstance();
            var dalconfig = new Com.Bekijkhet.MyRouter.DalPsql.DalConfig(Environment.GetEnvironmentVariable("MYROUTER_DB"));
            container.Register<Com.Bekijkhet.MyRouter.DalPsql.DalConfig>(dalconfig);
            container.Register<IDal, Com.Bekijkhet.MyRouter.DalPsql.Dal>().AsMultiInstance();
            container.Register<IBrokerClient, Com.Bekijkhet.MyRouter.BrokerClientImpl.BrokerClient>().AsMultiInstance();
            container.Register<IProcessor, ProcessorImpl> ().AsMultiInstance();
            Log.Info(log, "Starting MyRouter...", now);
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
                        Log.Error(log, e.Message, now, e);
                    }
                }
            }
        }
    }
}
