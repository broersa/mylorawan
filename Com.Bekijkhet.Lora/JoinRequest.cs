using System;

namespace Com.Bekijkhet.Lora
{
    public class JoinRequest
    {
        public Mhdr Mhdr { get; set;}
        public byte[] AppEUI { get; set;}
        public byte[] DevEUI { get; set;}
        public byte[] DevNonce { get; set;}
        public byte[] Mic { get; set;}
    }
}

