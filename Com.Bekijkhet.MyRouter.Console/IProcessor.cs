using System;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Com.Bekijkhet.MyRouter.Console
{
    public interface IProcessor
    {
        Task Process (UdpReceiveResult message, UdpClient client);
    }
}

