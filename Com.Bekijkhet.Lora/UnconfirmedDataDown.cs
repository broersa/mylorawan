using System;

namespace Com.Bekijkhet.Lora
{
    public class UnconfirmedDataDown
    {
        public Mhdr Mhdr { get; set;}
        public FhdrDownlink Fhdr { get; set;}
        public byte FPort { get; set;}
        public byte[] FRMPayload { get; set;}
        public byte[] Mic { get; set;}
    }
}

