using System;
using System.Threading.Tasks;

namespace Com.Bekijkhet.MyBroker.Bll
{
    public interface IBll
    {
        Task<byte[]> ProcessJoinRequest(byte[] data);

    }
}

