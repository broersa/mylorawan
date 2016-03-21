using System;

namespace Com.Bekijkhet.Semtech
{
    public class PushData
    {
        public byte ProtocolVersion { get; set;}
        public byte[] RandomToken { get; set;}
        public Identifier Identifier { get; set; }
        public byte[] GatewayMACAddress { get; set; }
        public PushDataJson Json {get;set;}
    }
}

