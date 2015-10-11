using System;

namespace Twitch.Api.Model.StreamElements
{
    public class StreamStream
    {
        public string Game { set; get; }
        public int Viewers { set; get; }
        public float Average_Fps { set; get; }
        public int Video_Height { set; get; }
        public DateTime Created_At { set; get; }
        public long _Id { set; get; }
        public Channel Channel { set; get; }
        public StreamStreamLinks _Links { set; get; }
    }
}
