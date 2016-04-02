using System;

namespace Com.Bekijkhet.Lora
{
    public class FhdrUplink
    {
        public DevAddr DevAddr { get; set;}
        public FCtrlUplink FCtrl { get; set; }
        public ushort FCnt {get;set;}
        public byte[] FOpts {get;set;}
    }
}

