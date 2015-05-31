using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Twitch.Api.Model;

namespace Twitch.Api
{
    public class OAuth
    {
        public readonly OAuthHttpServer HttpServer;
        public Thread HttpServerThread; 

        public OAuth()
        {
            HttpServer = new OAuthHttpServer(8080);
        }

        public void GotoAuthorization()
        {
            Process.Start("https://api.twitch.tv/kraken/oauth2/authorize" +
                   "?response_type=code" +
                   "&client_id=" + OAuthInfo.OAuthInfo.ClientId +
                   "&redirect_uri=http://localhost:8080");
        }

        public void GetAuthLink()
        {
            HttpServerThread = new Thread(HttpServer.listen);
            HttpServerThread.Start();
        }

        public async Task<string> GetAccessToken(string authCode)
        {           
            var client = new HttpClient { BaseAddress = new Uri("https://api.twitch.tv/kraken/oauth2/token") };

            var uri = "?client_id=" + OAuthInfo.OAuthInfo.ClientId +
                      "&client_secret=" + OAuthInfo.OAuthInfo.Secret +
                      "&grant_type=authorization_code" +
                      "&redirect_uri=http://localhost:8080" +
                      "&code=" + authCode;

            var content = new FormUrlEncodedContent(new[] 
            {
                new KeyValuePair<string, string>("", "login")
            });

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.PostAsync(uri, content);
            var message = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var token = JsonConvert.DeserializeObject<OAuthAccessToken>(message);
                return token.Access_Token;
            }

            var error = JsonConvert.DeserializeObject<QueryError>(message);
            throw new Exception(error.Error + ": " + error.Message);
        }
    }
}
