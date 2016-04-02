using System;
using System.Threading.Tasks;
using Com.Bekijkhet.Logger;
using System.Net.Sockets;
using Com.Bekijkhet.Semtech;
using System.Collections.Concurrent;
using System.Net;
using System.Text;
using Com.Bekijkhet.Lora;
using Com.Bekijkhet.MyRouter.BrokerClient;
//using Com.Bekijkhet.MyRouter.Dal;
using log4net;

namespace Com.Bekijkhet.MyRouter.Console
{
    public class ProcessorImpl : IProcessor
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static ConcurrentDictionary<string, IPEndPoint> _gateways = new ConcurrentDictionary<string, IPEndPoint>();

        private ISemtech _semtech;
        private ILora _lora;
        private IBrokerClient _brokerclient;

        public ProcessorImpl (ISemtech semtech, ILora lora, IBrokerClient brokerclient)
        {
            _semtech = semtech;
            _lora = lora;
            _brokerclient = brokerclient;
        }

        #region IProcessor implementation
        public async Task Process (UdpReceiveResult message, UdpClient client)
        {
            var now = DateTime.UtcNow;
            try {
                switch (_semtech.GetIdentifier(message.Buffer))
                {
                case Identifier.PUSH_DATA:
                    Log.Info(log, "PUSH_DATA from " + message.RemoteEndPoint.Address.ToString() + ":" +message.RemoteEndPoint.Port.ToString(), now);
                    var pushdata = _semtech.UnmarshalPushData(message.Buffer);
                    IPEndPoint ep = null;
                    Broker broker = null;
                    if (_gateways.TryGetValue(ByteArrayToString(pushdata.GatewayMACAddress), out ep))
                    {
                        var pushack = _semtech.MarshalPushAck(pushdata.RandomToken); 
                        client.SendAsync(pushack, pushack.Length, ep);
                        Log.Info(log, "PUSH_ACK to " + ep.Address.ToString() + ":" +ep.Port.ToString(), now);

                        foreach (var rxpk in pushdata.Json.Rxpks) {
                            var data = Convert.FromBase64String(rxpk.Data);
                            switch (_lora.GetMType(data[0])) {
                            case MType.JoinRequest:
                                var joinrequest = _lora.UnmarshalJoinRequest(data);
                                broker = await _brokerclient.GetBrokerOnAppEUI(ByteArrayToString(joinrequest.AppEUI));
                                var joinaccept = await _brokerclient.SendMessage(broker.Endpoint, new Message() { Rxpk = rxpk } );
                                var pullresp = _semtech.MarshalPullResp(new PullResp() {
                                    ProtocolVersion = 1,
                                    Identifier = Identifier.PULL_RESP,
                                    Txpk = joinaccept.Txpk
                                });
                                client.SendAsync(pullresp, pullresp.Length, ep);
                                Log.Info(log, "PULL_RESP to " + ep.Address.ToString() + ":" +ep.Port.ToString(), now);
                                break;
                            case MType.ConfirmedDataUp:
                                // TODO
                                var confirmeddataup = _lora.UnmarshalUnconfirmedDataUp(data);
                                broker = await _brokerclient.GetBrokerOnDevAddr(ByteArrayToString(_lora.MarshalDevAddr(confirmeddataup.Fhdr.DevAddr)));
                                await _brokerclient.SendMessage(broker.Endpoint, new Message() { Rxpk = rxpk } );
                                break;
                            case MType.UnconfirmedDataUp:
                                var unconfirmeddataup = _lora.UnmarshalUnconfirmedDataUp(data);
                                broker = await _brokerclient.GetBrokerOnDevAddr(ByteArrayToString(_lora.MarshalDevAddr(unconfirmeddataup.Fhdr.DevAddr)));
                                await _brokerclient.SendMessage(broker.Endpoint, new Message() { Rxpk = rxpk } );
                                break;
                            }
                        }
                    }
                    break;
                case Identifier.PULL_DATA:
                    Log.Info(log, "PULL_DATA from " + message.RemoteEndPoint.Address.ToString() + ":" +message.RemoteEndPoint.Port.ToString(), now);
                    var pulldata = _semtech.UnmarshalPullData(message.Buffer);
                    _gateways[ByteArrayToString(pulldata.GatewayMACAddress)]=message.RemoteEndPoint;
                    var pullack = _semtech.MarshalPullAck(pulldata.RandomToken); 
                    client.SendAsync(pullack, pullack.Length, message.RemoteEndPoint);
                    Log.Info(log, "PULL_ACK to " + message.RemoteEndPoint.Address.ToString() + ":" +message.RemoteEndPoint.Port.ToString(), now);
                    break;
                }
            }
            catch (Exception e) {
                Log.Error(log, e.Message, now, e);
            }
        }
        #endregion

        private static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        private static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

    }
}

