using System;
using Nancy;
using Nancy.Responses;
using Newtonsoft.Json;
using Com.Bekijkhet.Lora;
using Com.Bekijkhet.MyBroker.Dal;
using Com.Bekijkhet.MyBroker.Bll;

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
                try {
                    var message = JsonConvert.DeserializeObject<Message>(Request.Body.ToString());
                    var data = Convert.FromBase64String(message.Rxpk.Data);
                    switch (lora.GetMType(data[0])) {
                    case MType.JoinRequest:

                        break;
                    case MType.ConfirmedDataDown:

                        break;
                    case MType.UnconfirmedDataDown:

                        break;
                    }
                    return Response.AsJson(new ReturnMessage());
                }
                catch (Exception e)
                {
                    throw;
                }
            };
        }

    }
}

