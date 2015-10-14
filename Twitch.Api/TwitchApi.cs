using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Api.Core;
using Api.Twitch.Model;
using Newtonsoft.Json;

namespace Api.Twitch
{
    public class TwitchApi
    {
        private readonly string _channel;
        private readonly string _oAuthKey;
        private readonly string _apiBaseUri = "https://api.twitch.tv/kraken/";

        public TwitchApi(string channel, string oAuthKey)
        {
            _channel = channel;
            _oAuthKey = oAuthKey;
        }

        public async Task<Channel> GetChannel()
        {
            return JsonConvert.DeserializeObject<Channel>(await HttpRequester.Get(_apiBaseUri, "channels/" + _channel));
        }

        public async Task<StreamInfo> GetStream()
        {
            return JsonConvert.DeserializeObject<StreamInfo>(await HttpRequester.Get(_apiBaseUri, "streams/" + _channel));
        }

        public async Task<SearchGames> SearchGames(string query)
        {
            return JsonConvert.DeserializeObject<SearchGames>(await HttpRequester.Get(_apiBaseUri, "search/games?q=" + query + "&type=suggest"));
        }

        public async Task<bool> UpdateStatus(string status)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("channel[status]", status), 
            });

            return await HttpRequester.Put(_apiBaseUri, "channels/" + _channel + "?oauth_token=" + _oAuthKey, content);
        }

        public async Task<bool> UpdateGame(string game)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("channel[game]", game), 
            });

            return await HttpRequester.Put(_apiBaseUri, "channels/" + _channel + "?oauth_token=" + _oAuthKey, content);
        }
    }
}
