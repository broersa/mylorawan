
using System;
using Newtonsoft.Json;
using Com.Bekijkhet.Semtech;

namespace Com.Bekijkhet.MyRouter.BrokerClientImpl
{
    public class Message
    {
        [JsonProperty("rxpk")]
        public Rxpk Rxpk {get;set;}
    }
}

