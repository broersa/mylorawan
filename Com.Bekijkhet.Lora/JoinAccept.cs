using System;

namespace Com.Bekijkhet.Lora
{
    public class JoinAccept
    {
        public Mhdr Mhdr { get; set; }
        public byte[] AppNonce { get; set; }
        public byte[] NetId { get; set; }
        public UInt32 DevAddr { get; set; }
        public byte DlSettings { get; set; }
        public byte RxDelay { get; set; }
        public byte[] CfList { get; set; }
        public byte[] Mic{ get; set; }
    }
}

