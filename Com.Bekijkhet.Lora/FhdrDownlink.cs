using System;

namespace Com.Bekijkhet.Lora
{
    public class FhdrDownlink
    {
        public byte[] DevAddr { get; set;}
        public FCtrlDownlink FCtrl { get; set; }
        public ushort FCnt {get;set;}
        public byte[] FOpts {get;set;}
    }
}

