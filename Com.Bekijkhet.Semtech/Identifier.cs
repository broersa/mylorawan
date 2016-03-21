using System;

namespace Com.Bekijkhet.Semtech
{
    public enum Identifier
    {
        PUSH_DATA = 0x00,
        PUSH_ACK = 0x01,
        PULL_DATA = 0x02,
        PULL_RESP = 0x03,
        PULL_ACK = 0x04
    }
}

