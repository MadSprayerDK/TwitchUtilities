using Api.Twitch.Model.Links;
using Api.Twitch.Model.StreamElements;

namespace Api.Twitch.Model
{
    public class StreamInfo
    {
        public StreamLinks _Links { set; get; }
        public StreamStream Stream { set; get; }
    }
}
