﻿using System;

namespace Com.Bekijkhet.Lora
{
    public class UnconfirmedDataUp
    {
        public Mhdr Mhdr { get; set;}
        public FhdrUplink Fhdr { get; set;}
        public byte FPort { get; set;}
        public byte[] FRMPayload { get; set;}
        public byte[] Mic { get; set;}
    }
}

