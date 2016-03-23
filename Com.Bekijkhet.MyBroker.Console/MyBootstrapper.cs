using System;
using Nancy;
using Nancy.TinyIoc;
using Com.Bekijkhet.Lora;
using Com.Bekijkhet.MyBroker.Dal;
using Com.Bekijkhet.MyBroker.Bll;

namespace Com.Bekijkhet.MyBroker
{
    public class MyBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            // Autoregister will actually do this for us, so we don't need this line,
            // but I'll keep it here to demonstrate. By Default anything registered
            // against an interface will be a singleton instance.
            container.Register<ILora, LoraImpl>().AsMultiInstance();
            var dalconfig = new Com.Bekijkhet.MyBroker.DalPsql.DalConfig(Environment.GetEnvironmentVariable("MYBROKER_DB"));
            container.Register<Com.Bekijkhet.MyBroker.DalPsql.DalConfig>(dalconfig);
            container.Register<IDal, Com.Bekijkhet.MyBroker.DalPsql.Dal>().AsMultiInstance();
            container.Register<IBll, Com.Bekijkhet.MyBroker.BllImpl.Bll>().AsMultiInstance();
        }
    }
}

