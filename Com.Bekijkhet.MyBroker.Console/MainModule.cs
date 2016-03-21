using System;
using Nancy;
using Nancy.Responses;
using Newtonsoft.Json;

namespace Com.Bekijkhet.MyBroker.Console
{
    public class MainModule : NancyModule
    {
        public MainModule()
        {
            Get["/", runAsync: true] = async (_, token) =>
            {
                return "Hello World!";
            };
            Get["/hasappeui/{appeui}", runAsync: true] = async (parameters, token) => {
                return Response.AsJson(true);
            };
            Post["/message", runAsync: true] = async (parameters, token) => {
                var message = JsonConvert.DeserializeObject<Message>(Request.Body.ToString());

                return Response.AsJson(new ReturnMessage());
            };
        }
    }
}

