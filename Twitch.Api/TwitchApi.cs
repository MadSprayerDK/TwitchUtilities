using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Twitch.Api.Model;

namespace Twitch.Api
{
    public class TwitchApi
    {
        public string Channel { set; get; }
        private string ApiBaseUri = "https://api.twitch.tv/kraken/";

        public TwitchApi()
        { }

        public TwitchApi(string channel)
        {
            Channel = channel;
        }

        public Channel GetChannel()
        {
            return JsonConvert.DeserializeObject<Channel>(PerformHttpsRequest("channels/" + Channel));
        }

        public Stream GetStream()
        {
            return JsonConvert.DeserializeObject<Stream>(PerformHttpsRequest("streams/" + Channel));
        }

        private string PerformHttpsRequest(string uri)
        {
            if(string.IsNullOrEmpty(Channel))
                throw new Exception("No channel is set");

            return "";
        }
    }
}
