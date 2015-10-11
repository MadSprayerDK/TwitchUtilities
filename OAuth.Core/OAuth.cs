using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using OAuth.Core.HttpServer;
using OAuth.Core.Model;
using OAuth.Core.Providers;

namespace OAuth.Core
{
    public class OAuth
    {
        public delegate void RecievedCode(object sender, StringEventArg args);
        public event RecievedCode OAuthCodeRecived;

        private readonly IOAuthProvider _provider;

        private readonly OAuthHttpServer _httpServer;
        private Thread _httpServerThread;

        public OAuth(IOAuthProvider provider)
        {
            _provider = provider;

            _httpServer = new OAuthHttpServer(8080, _provider.GetSuccessUri());
            _httpServer.OAuthCodeRecived += _httpServer_OAuthCodeRecived;
        }

        void _httpServer_OAuthCodeRecived(object sender, StringEventArg args)
        {
            StopOAuthRedirectServer();

            if (OAuthCodeRecived != null)
                OAuthCodeRecived(sender, args);
        }

        public void GotoAuthorization()
        {
            Process.Start(_provider.GetAuthUri());
        }

        public void StartOAuthRedirectServer()
        {
            _httpServerThread = new Thread(_httpServer.Listen);
            _httpServerThread.Start();
        }

        public void StopOAuthRedirectServer()
        {
            _httpServer.Stop();
        }

        public async Task<string> GetAccessToken(string oauthCode)
        {
            var client = new HttpClient { BaseAddress = new Uri(_provider.GetBaseUri() + _provider.GetTokenQueryUri()) };

            var uri = _provider.GetAccessTokenUri(oauthCode);

            var content = new FormUrlEncodedContent(new[] 
            {
                new KeyValuePair<string, string>("", "")
            });

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.PostAsync(uri, content);
            var message = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
                return _provider.ExtractAccessCode(message);

            throw new Exception(_provider.ExtractErrorMessage(message));
        }

        public async Task<TokenInfo> VerifyToken(string oAuthToken)
        {
            string uri;
            try
            {
                uri = _provider.GetValidateTokenUri(oAuthToken);
            }
            catch (NotImplementedException)
            {
                return null;
            }

            var client = new HttpClient { BaseAddress = new Uri(_provider.GetBaseUri()) };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.GetAsync(uri);
            var message = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return _provider.ExtractVerifiedMessage(message);
            }

            throw new Exception(_provider.ExtractErrorMessage(message));
        }
    }
}