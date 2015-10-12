using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Twitch.Api.Model;

namespace Twitch.Api
{
    public class TwitchApi
    {
        private string Channel { set; get; }
        private string OAuthKey { set; get; }
        private string ApiBaseUri = "https://api.twitch.tv/kraken/";

        public TwitchApi(string channel, string oAuthKey)
        {
            Channel = channel;
            OAuthKey = oAuthKey;
        }

        public async Task<Channel> GetChannel()
        {
            return JsonConvert.DeserializeObject<Channel>(await PerformHttpsRequest("channels/" + Channel));
        }

        public async Task<StreamInfo> GetStream()
        {
            return JsonConvert.DeserializeObject<StreamInfo>(await PerformHttpsRequest("streams/" + Channel));
        }

        public async Task<SearchGames> SearchGames(string query)
        {
            return
                JsonConvert.DeserializeObject<SearchGames>(
                    await PerformHttpsRequest("search/games?q=" + query + "&type=suggest"));
        }

        private async Task<string> PerformHttpsRequest(string uri)
        {
            var client = new HttpClient {BaseAddress = new Uri(ApiBaseUri)};

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.GetAsync(uri);
            var message = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
                return message;
            
            var error = JsonConvert.DeserializeObject<QueryError>(message);
            throw new Exception(error.Error + ": " + error.Message);
        }

        public async Task<bool> UpdateStatus(string status)
        {
            return await PutHttpRequest("channels/" + Channel, "status", status);
        }

        public async Task<bool> UpdateGame(string game)
        {
            return await PutHttpRequest("channels/" + Channel, "game", game);
        }

        private async Task<bool> PutHttpRequest(string uri, string type, string argument)
        {
            var client = new HttpClient(){BaseAddress = new Uri(ApiBaseUri)};

            var content = new FormUrlEncodedContent(new[] 
            {
                new KeyValuePair<string, string>("channel["+type+"]", argument),
            });

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

            var response = await client.PutAsync(uri + "?oauth_token=" + OAuthKey, content);

            if (response.StatusCode == HttpStatusCode.OK)
                return true;

            var error = JsonConvert.DeserializeObject<QueryError>(await response.Content.ReadAsStringAsync());
            throw new Exception(error.Error + ": " + error.Message);
        }
    }
}
