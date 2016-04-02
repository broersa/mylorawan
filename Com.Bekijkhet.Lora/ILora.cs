using System;

namespace Com.Bekijkhet.Lora
{
    public interface ILora
    {
        MType GetMType(byte mhdr);
        JoinRequest UnmarshalJoinRequest(byte[] message);
        JoinRequest UnmarshalJoinRequestAndValidate(byte[] appkey, byte[] message);
        byte[] MarshalJoinAccept(JoinAccept joinaccept, byte[] appkey);
        UnconfirmedDataUp UnmarshalUnconfirmedDataUp(byte[] message);
        UnconfirmedDataUp UnmarshalUnconfirmedDataUpAndValidate(byte[] nwkskey, byte[] message);
        byte[] DecryptFRMPayload(byte[] key, UnconfirmedDataUp data);

        byte[] GetAppNonce();
        byte[] GetNwkSKey(byte[] appkey, byte[] appnonce, byte[] netid, byte[] devnonce);
        byte[] GetAppSKey(byte[] appkey, byte[] appnonce, byte[] netid, byte[] devnonce);
        byte[] MarshalDevAddr(DevAddr devaddr);
    }
}

