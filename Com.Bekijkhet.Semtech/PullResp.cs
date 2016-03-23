using System;

namespace Com.Bekijkhet.Semtech
{
    public class PullResp
    {
        public byte ProtocolVersion { get; set;}
        public Identifier Identifier { get; set; }
        public Txpk Txpk { get; set; }
    }
}

