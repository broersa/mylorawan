using System;
using Newtonsoft.Json;
using Com.Bekijkhet.Semtech;

namespace Com.Bekijkhet.MyRouter.BrokerClientImpl
{
    public class ReturnMessage
    {
        [JsonProperty("Txpk")]
        public Txpk Txpk {get;set;}
    }
}

