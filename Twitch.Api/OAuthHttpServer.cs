using System;
using System.IO;
using System.Linq;
using Twitch.Api.HttpServer;

namespace Twitch.Api
{
    public class OAuthHttpServer : HttpServer.HttpServer
    {
        public delegate void RecivedCode(object sender, StringEventArg args);
        public event RecivedCode OAuthCodeRecived;

        public OAuthHttpServer(int port) : base(port)
        {}

        public override void handleGETRequest(HttpProcessor p)
        {
            var noPrefix = p.http_url.Substring(p.http_url.IndexOf("?", StringComparison.Ordinal) + 1);
            var queryElements = noPrefix.Split('&').Select(x => x.Split('=')).ToDictionary(x => x[0], x => x[1]);
            if(OAuthCodeRecived != null)
                OAuthCodeRecived(this, new StringEventArg(queryElements["code"]));

            p.writeSuccess();
            p.outputStream.WriteLine("<html><body><h1>Success</h1>");
            p.outputStream.WriteLine("You can now close this page.");
        }

        public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            is_active = false;
            listener.Stop();
        }
    }
}
