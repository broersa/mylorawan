using System;

namespace Com.Bekijkhet.Lora
{
    public enum MType
    {
        JoinRequest = 0x00,
        JoinAccept = 0x01,
        UnconfirmedDataUp = 0x02,
        UnconfirmedDataDown = 0x03,
        ConfirmedDataUp = 0x04,
        ConfirmedDataDown = 0x05
    }
}

