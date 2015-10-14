using System;
using Api.Twitch.Model.Links;

namespace Api.Twitch.Model
{
    public class Channel
    {
        public string Mature { set; get; }
        public string Status { set; get; }
        public string Broadcaster_Language { set; get; }
        public string Display_Name { set; get; }
        public string Game { set; get; }
        public int? Delay { set; get; }
        public string Language { set; get; }
        public int _Id { set; get; }
        public string Name { set; get; }
        public DateTime Created_At { set; get; }
        public DateTime Updated_At { set; get; }
        public string Logo { set; get; }
        public string Banner { set; get; }
        public string Video_Banner { set; get; }
        public string Background { set; get; }
        public string Profile_Banner { set; get; }
        public string Profile_Banner_Background_Color { set; get; }
        public bool Partner { set; get; }
        public string Url { set; get; }
        public int Views { set; get; }
        public int Followers { set; get; }
        public ChannelLinks _Links { set; get; } 
    }
}
