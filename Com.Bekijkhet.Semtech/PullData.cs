﻿using System;

namespace Com.Bekijkhet.Semtech
{
    public class PullData
    {
        public byte ProtocolVersion { get; set;}
        public byte[] RandomToken { get; set;}
        public Identifier Identifier { get; set; }
        public byte[] GatewayMACAddress { get; set; }
    }
}

