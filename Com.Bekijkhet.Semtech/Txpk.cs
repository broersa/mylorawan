using System;
using Newtonsoft.Json;

namespace Com.Bekijkhet.Semtech
{
    public class Txpk
    {
        [JsonProperty("imme")]
        public bool Immediate { get; set; }
        [JsonProperty("tmst")]
        public UInt32 Tmst { get; set; }
        [JsonProperty("time")]
        public string Time { get; set; }
        [JsonProperty("freq")]
        public float Freq { get; set; }
        [JsonProperty("rfch")]
        public UInt16 RfCh { get; set; }
        [JsonProperty("powe")]
        public UInt16 Power { get; set; }
        [JsonProperty("modu")]
        public string Modulation { get; set; }
        [JsonProperty("datr")]
        public string DataRate {get;set;}
        [JsonProperty("codr")]
        public string CodingRate {get;set;}
        [JsonProperty("fdev")]
        public UInt16 FrequencyDeviation {get;set;}
        [JsonProperty("ipol")]
        public bool Polarization {get;set;}
        [JsonProperty("prea")]
        public UInt16 Preamble {get;set;}
        [JsonProperty("size")]
        public UInt16 Size {get;set;}
        [JsonProperty("data")]
        public string Data { get; set;}
        [JsonProperty("ncrc")]
        public bool NoCRC { get; set;}
    }
}

