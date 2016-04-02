using System;
using Newtonsoft.Json;
using Com.Bekijkhet.Semtech;

namespace Com.Bekijkhet.MyBroker.Console
{
    public class ReturnMessage
    {
        [JsonProperty("txresult")]
        public bool TxResult { get; set; }
        [JsonProperty("txpk")]
        public Txpk Txpk {get;set;}
    }
}

