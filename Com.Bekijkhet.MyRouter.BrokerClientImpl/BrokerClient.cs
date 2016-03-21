using System;
using Com.Bekijkhet.MyRouter.BrokerClient;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Com.Bekijkhet.Semtech;
using Com.Bekijkhet.MyRouter.Dal;

namespace Com.Bekijkhet.MyRouter.BrokerClientImpl
{
    public class BrokerClient : IBrokerClient
    {
        private IDal _dal;

        public BrokerClient(IDal dal)
        {
            _dal = dal;
        }

        #region IBrokerClient implementation

        public async Task<Com.Bekijkhet.MyRouter.BrokerClient.Broker> GetBrokerOnAppEUI(string appeui)
        {
            _dal.BeginTransaction();
            foreach (var b in await _dal.GetBrokers()) {
                Console.WriteLine(b.Name);
            }

            return new Com.Bekijkhet.MyRouter.BrokerClient.Broker() {
                AppEUI = appeui,
                Endpoint = "http://localhost:8888"
            };
        }

        public async Task<Txpk> SendMessage(string endpoint, Rxpk rxpk)
        {
            ReturnMessage returnmessage = null; 
            var message = new Message() { 
                Rxpk = rxpk
            };
            using (var h = new HttpClient()) {
                var r = await h.PostAsync(endpoint+"/message", new StringContent(JsonConvert.SerializeObject(message)));
                if (!r.IsSuccessStatusCode) {
                    throw new SendMessageException();
                }
                returnmessage = JsonConvert.DeserializeObject<ReturnMessage>(await r.Content.ReadAsStringAsync());
            }
            return returnmessage.Txpk;
        }

        #endregion
    }
}

