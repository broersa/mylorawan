using System;

namespace Com.Bekijkhet.Lora
{
    public class FCtrlDownlink
    {
        public bool ADR {get;set;}
        public bool ADRACKReq {get;set;}
        public bool ACK {get;set;}
        public bool FPending {get;set;}
        public byte FOptsLen {get;set;}
    }
}

