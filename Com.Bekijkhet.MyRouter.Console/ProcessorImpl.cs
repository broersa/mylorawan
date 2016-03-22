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
using Com.Bekijkhet.MyRouter.Dal;

namespace Com.Bekijkhet.MyRouter.Console
{
    public class ProcessorImpl : IProcessor
    {
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
            var bx = await _brokerclient.GetBrokerOnAppEUI("");
            var now = DateTime.UtcNow;
            try {
                switch (_semtech.GetIdentifier(message.Buffer))
                {
                case Identifier.PUSH_DATA:
                    var pushdata = _semtech.UnmarshalPushData(message.Buffer);
                    IPEndPoint ep = null;
                    if (_gateways.TryGetValue(ByteArrayToString(pushdata.GatewayMACAddress), out ep))
                    {
                        var pushack = _semtech.MarshalPushAck(pushdata.RandomToken); 
                        client.SendAsync(pushack, pushack.Length, ep);
                        foreach (var rxpk in pushdata.Json.Rxpks) {
                            var data = Convert.FromBase64String(rxpk.Data);
                            switch (_lora.GetMType(data[0])) {
                            case MType.JoinRequest:
                                var joinrequest = _lora.UnmarshalJoinRequest(data);
                                var broker = await _brokerclient.GetBrokerOnAppEUI(ByteArrayToString(joinrequest.AppEUI));
                                var s = await _brokerclient.SendMessage(broker.Endpoint, rxpk);

                                break;
                            case MType.ConfirmedDataDown:

                                break;
                            case MType.UnconfirmedDataDown:

                                break;
                            }
                        }
                    }
                    break;
                case Identifier.PULL_DATA:
                    var pulldata = _semtech.UnmarshalPullData(message.Buffer);
                    _gateways[ByteArrayToString(pulldata.GatewayMACAddress)]=message.RemoteEndPoint;
                    var pullack = _semtech.MarshalPullAck(pulldata.RandomToken); 
                    client.SendAsync(pullack, pullack.Length, message.RemoteEndPoint);
                    break;
                }
            }
            catch (Exception e) {
                Log.Error (e, now);
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

