using System;

namespace Com.Bekijkhet.Semtech
{
    public interface ISemtech
    {
        Identifier GetIdentifier(byte[] packet);
        PushData UnmarshalPushData(byte[] packet);
        PullData UnmarshalPullData(byte[] packet);
        byte[] MarshalPushAck(byte[] randomtoken);
        byte[] MarshalPullAck(byte[] randomtoken);
        //byte[] Marshall
    }
}

