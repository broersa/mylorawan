using System;
using Nancy;
using Nancy.Responses;
using Nancy.Extensions;
using Newtonsoft.Json;
using Com.Bekijkhet.Lora;
using Com.Bekijkhet.MyBroker.Dal;
using Com.Bekijkhet.MyBroker.Bll;
using Com.Bekijkhet.Semtech;
using log4net;

namespace Com.Bekijkhet.MyBroker.Console
{
    public class MainModule : NancyModule
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public MainModule(ILora lora, IBll bll)
        {
            Get["/", true] = async (_, ct) =>
            {
                return "Hello World!";
            };
            Get["/hasappeui/{appeui}", true] = async (parameters, ct) =>
            {
                return Response.AsJson(true);
                //return HttpStatusCode.NoResponse;
            };
            Post["/message", true] = async (parameters, ct) => 
            {
                ReturnMessage returnmessage = null;
                try {
                    var message = JsonConvert.DeserializeObject<Message>(Request.Body.AsString());
                    var data = Convert.FromBase64String(message.Rxpk.Data);
                    switch (lora.GetMType(data[0])) {
                    case MType.JoinRequest:
                        log.Info("JoinRequest");
                        var joinaccept = await bll.ProcessJoinRequest(data);
                        returnmessage = new ReturnMessage() {
                            TxResult = true,
                            Txpk = new Txpk() {
                                Tmst = message.Rxpk.Tmst + 5000000,
                                Freq = message.Rxpk.Freq,
                                RfCh = message.Rxpk.RfCh,
                                Power = 14,
                                Modulation = message.Rxpk.Modulation,
                                DataRate = message.Rxpk.DataRate,
                                CodingRate = message.Rxpk.CodingRate,
                                Polarization = true,
                                Size = Convert.ToUInt16(joinaccept.Length - 4),
                                Data = Convert.ToBase64String(joinaccept)
                            }
                        };
                        return Response.AsText(JsonConvert.SerializeObject(returnmessage), "application/json");
                    case MType.ConfirmedDataUp:
                        log.Info("ConfirmedDataUp");
                        return Response.AsText(JsonConvert.SerializeObject(new ReturnMessage() { TxResult = false }), "application/json");
                    case MType.UnconfirmedDataUp:
                        log.Info("UnconfirmedDataUp");
                        var unconfirmeddataup = await bll.ProcessUnconfirmedDataUp(data);
                        return Response.AsText(JsonConvert.SerializeObject(new ReturnMessage() { TxResult = false }), "application/json");
                    }
                    log.Info("Unknown MType: " + lora.GetMType(data[0]));
                    return Response.AsText(JsonConvert.SerializeObject(new ReturnMessage() { TxResult = false }), "application/json");
                }
                catch (Exception e)
                {
                    log.Error(e.Message, e);
                    return Response.AsText(JsonConvert.SerializeObject(new ReturnMessage() { TxResult = false }), "application/json");
                }
            };
        }

    }
}

