using System;

namespace Com.Bekijkhet.Lora
{
    public interface ILora
    {
        MType GetMType(byte mhdr);
        JoinRequest UnmarshalJoinRequest(byte[] message);
    }
}

