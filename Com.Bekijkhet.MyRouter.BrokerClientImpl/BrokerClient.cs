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

        public async Task<Com.Bekijkhet.MyRouter.BrokerClient.ReturnMessage> SendMessage(string endpoint, Com.Bekijkhet.MyRouter.BrokerClient.Message message)
        {
            Com.Bekijkhet.MyRouter.BrokerClient.ReturnMessage returnmessage = null; 
            var msg = new Com.Bekijkhet.MyRouter.BrokerClientImpl.Message() { 
                Rxpk = message.Rxpk
            };
            using (var h = new HttpClient()) {
                var r = await h.PostAsync(endpoint+"/message", new StringContent(JsonConvert.SerializeObject(msg)));
                if (!r.IsSuccessStatusCode) {
                    throw new SendMessageException();
                }
                var rmsg = JsonConvert.DeserializeObject<Com.Bekijkhet.MyRouter.BrokerClientImpl.ReturnMessage>(await r.Content.ReadAsStringAsync());
                returnmessage = new Com.Bekijkhet.MyRouter.BrokerClient.ReturnMessage() {
                    Txpk = rmsg.Txpk
                };
            }
            return returnmessage;
        }

        #endregion
    }
}

