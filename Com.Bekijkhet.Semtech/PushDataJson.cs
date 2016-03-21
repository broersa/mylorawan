using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Com.Bekijkhet.Semtech
{
    public class PushDataJson
    {
        [JsonProperty("rxpk")]
        public List<Rxpk> Rxpks { get; set; }
    }
}

