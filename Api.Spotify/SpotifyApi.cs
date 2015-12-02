using System;
using System.Net.Http;
using System.Threading.Tasks;
using Api.Spotify.Responses;
using Newtonsoft.Json;

namespace Api.Spotify
{
    public class SpotifyApi
    {
        private string origin = "https://open.spotify.com";

        private readonly string _host;
        private string _oauthToken;
        private string _csrfToken;

        public SpotifyApi(string host)
        {
            _host = host;
        }

        public async Task<bool> Init()
        {
            await GetOauth();
            await GetCsrf();

            return true;
        }

        private async Task<bool> GetOauth()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("https://open.spotify.com")
            };

            var response = await client.GetAsync("/token");
            var message = await response.Content.ReadAsStringAsync();

            var token = JsonConvert.DeserializeObject<OauthToken>(message);
            _oauthToken = token.T;
            
            return true;
        }

        private async Task<bool> GetCsrf()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("https://" + _host + ":4370")
            };

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Origin", origin);

            var response = await client.GetAsync("/simplecsrf/token.json");
            var message = await response.Content.ReadAsStringAsync();

            var token = JsonConvert.DeserializeObject<CsrfToken>(message);

            _csrfToken = token.Token;

            return true;
        }

        public async Task<Status> GetStatus()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("https://" + _host + ":4370")
            };

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Origin", origin);

            var response = await client.GetAsync("/remote/status.json?csrf=" + _csrfToken + "&oauth=" + _oauthToken);
            var message = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Status>(message);
        }
    }
}
