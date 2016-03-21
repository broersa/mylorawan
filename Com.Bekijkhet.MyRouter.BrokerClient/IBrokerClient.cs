using System;
using System.Threading.Tasks;
using Com.Bekijkhet.Semtech;

namespace Com.Bekijkhet.MyRouter.BrokerClient
{
    public interface IBrokerClient
    {
        Task<Broker> GetBrokerOnAppEUI(string appeui);
        Task<Txpk> SendMessage(string endpoint, Rxpk rxpk);
    }
}

