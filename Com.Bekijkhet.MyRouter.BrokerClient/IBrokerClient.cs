using System;
using System.Threading.Tasks;
using Com.Bekijkhet.Semtech;

namespace Com.Bekijkhet.MyRouter.BrokerClient
{
    public interface IBrokerClient
    {
        Task<Broker> GetBrokerOnAppEUI(string appeui);
        Task<ReturnMessage> SendMessage(string endpoint, Message message);
    }
}

