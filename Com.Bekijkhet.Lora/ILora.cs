using System;

namespace Com.Bekijkhet.Lora
{
    public interface ILora
    {
        MType GetMType(byte mhdr);
        JoinRequest UnmarshalJoinRequest(byte[] message);
        JoinRequest UnmarshalJoinRequestAndValidate(byte[] appkey, byte[] message);
        byte[] MarshalJoinAccept(JoinAccept joinaccept, byte[] appkey);
        UnconfirmedDataDown UnmarshalUnconfirmedDataDown(byte[] message);
        byte[] GetAppNonce();
    }
}

