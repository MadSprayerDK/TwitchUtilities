using System;
using System.IO;
using System.Linq;

namespace OAuth.Core.HttpServer
{
    public class OAuthHttpServer : HttpServer
    {
        public delegate void RecivedCode(object sender, StringEventArg args);
        public event RecivedCode OAuthCodeRecived;

        private readonly string _successUri;

        public OAuthHttpServer(int port, string successUri) : base(port)
        {
            _successUri = successUri;
        }

        public override void HandleGetRequest(HttpProcessor p)
        {
            var noPrefix = p.HttpUrl.Substring(p.HttpUrl.IndexOf("?", StringComparison.Ordinal) + 1);
            var queryElements = noPrefix.Split('&').Select(x => x.Split('=')).ToDictionary(x => x[0], x => x[1]);
            if(OAuthCodeRecived != null)
                OAuthCodeRecived(this, new StringEventArg(queryElements["code"]));

            p.WriteSuccess();
            p.OutputStream.WriteLine(File.ReadAllText(_successUri));
        }

        public void Stop()
        {
            IsActive = false;
            Listener.Stop();
        }
    }
}
