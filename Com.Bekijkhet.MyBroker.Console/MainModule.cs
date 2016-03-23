﻿using System;
using Nancy;
using Nancy.Responses;
using Newtonsoft.Json;
using Com.Bekijkhet.Lora;
using Com.Bekijkhet.MyBroker.Dal;
using Com.Bekijkhet.MyBroker.Bll;
using Com.Bekijkhet.Semtech;

namespace Com.Bekijkhet.MyBroker.Console
{
    public class MainModule : NancyModule
    {
        public MainModule(ILora lora, IBll bll)
        {
            Get["/", runAsync: true] = async (_, token) =>
            {
                return "Hello World!";
            };
            Get["/hasappeui/{appeui}", runAsync: true] = async (parameters, token) => {
                return Response.AsJson(true);
            };
            Post["/message", runAsync: true] = async (parameters, token) => {
                ReturnMessage returnmessage = null;
                try {
                    var message = JsonConvert.DeserializeObject<Message>(Request.Body.ToString());
                    var data = Convert.FromBase64String(message.Rxpk.Data);
                    switch (lora.GetMType(data[0])) {
                    case MType.JoinRequest:
                            var joinaccept = await bll.ProcessJoinRequest(data);
                            returnmessage = new ReturnMessage() {
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
                        break;
                    case MType.ConfirmedDataDown:

                        break;
                    case MType.UnconfirmedDataDown:

                        break;
                    }
                    return Response.AsJson(returnmessage);
                }
                catch (Exception e)
                {
                    throw;
                }
            };
        }

    }
}

