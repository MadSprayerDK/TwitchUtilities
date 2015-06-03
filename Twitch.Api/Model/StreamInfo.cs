using Twitch.Api.Model.Links;
using Twitch.Api.Model.StreamElements;

namespace Twitch.Api.Model
{
    public class StreamInfo
    {
        public StreamLinks _Links { set; get; }
        public StreamStream Stream { set; get; }
    }
}
