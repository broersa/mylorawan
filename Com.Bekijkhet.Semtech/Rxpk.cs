using System;
using Newtonsoft.Json;

namespace Com.Bekijkhet.Semtech
{
    public class Rxpk
    {
        [JsonProperty("time")]
        public string Time { get; set; }
        [JsonProperty("tmst")]
        public UInt32 Tmst { get; set; }
        [JsonProperty("freq")]
        public float Freq { get; set; }
        [JsonProperty("chan")]
        public uint Chan { get; set; }
        [JsonProperty("rfch")]
        public uint RfCh { get; set; }
        [JsonProperty("stat")]
        public byte CRCStatus { get; set; }
        [JsonProperty("modu")]
        public string Modulation { get; set; }
        [JsonProperty("datr")]
        public string DataRate {get;set;}
        [JsonProperty("codr")]
        public string CodingRate {get;set;}
        [JsonProperty("rssi")]
        public Int32 Rssi {get;set;}
        [JsonProperty("lsnr")]
        public float LoraSNR {get;set;}
        [JsonProperty("size")]
        public uint Size {get;set;}
        [JsonProperty("data")]
        public string Data { get; set;}
    }
}

