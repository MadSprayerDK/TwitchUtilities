using System;
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
        private string ApiBaseUri = "https://api.twitch.tv/kraken/";

        public TwitchApi(string channel)
        {
            Channel = channel;
        }

        public async Task<Channel> GetChannel()
        {
            return JsonConvert.DeserializeObject<Channel>(await PerformHttpsRequest("channels/" + Channel));
        }

        public async Task<StreamInfo> GetStream()
        {
            return JsonConvert.DeserializeObject<StreamInfo>(await PerformHttpsRequest("streams/" + Channel));
        }

        private async Task<string> PerformHttpsRequest(string uri)
        {
            if(string.IsNullOrEmpty(Channel))
                throw new Exception("No channel is set");

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
    }
}
