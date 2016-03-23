using System;
using Newtonsoft.Json;
using Com.Bekijkhet.Semtech;

namespace Com.Bekijkhet.MyRouter.BrokerClient
{
    public class Message
    {
        [JsonProperty("rxpk")]
        public Rxpk Rxpk {get;set;}
    }
}

