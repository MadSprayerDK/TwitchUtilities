using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
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

        public async Task<Stream> GetStream()
        {
            return JsonConvert.DeserializeObject<Stream>(await PerformHttpsRequest("streams/" + Channel));
        }

        private async Task<string> PerformHttpsRequest(string uri)
        {
            if(string.IsNullOrEmpty(Channel))
                throw new Exception("No channel is set");

            var client = new HttpClient {BaseAddress = new Uri(ApiBaseUri)};

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            string response;

            try
            {
                response = await client.GetStringAsync(uri);
            }
            catch (Exception e)
            {
                throw e;
            }

            return response;
        }


    }
}
