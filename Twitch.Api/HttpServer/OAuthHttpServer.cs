using System;
using System.IO;
using System.Linq;

namespace Twitch.Api.HttpServer
{
    public class OAuthHttpServer : HttpServer
    {
        public delegate void RecivedCode(object sender, StringEventArg args);
        public event RecivedCode OAuthCodeRecived;

        public OAuthHttpServer(int port) : base(port)
        {}

        public override void HandleGetRequest(HttpProcessor p)
        {
            var noPrefix = p.HttpUrl.Substring(p.HttpUrl.IndexOf("?", StringComparison.Ordinal) + 1);
            var queryElements = noPrefix.Split('&').Select(x => x.Split('=')).ToDictionary(x => x[0], x => x[1]);
            if(OAuthCodeRecived != null)
                OAuthCodeRecived(this, new StringEventArg(queryElements["code"]));

            p.WriteSuccess();
            p.OutputStream.WriteLine(File.ReadAllText("HttpServer/success.html"));
        }

        public void Stop()
        {
            IsActive = false;
            Listener.Stop();
        }
    }
}
