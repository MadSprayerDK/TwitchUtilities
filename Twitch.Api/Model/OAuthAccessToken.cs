namespace Twitch.Api.Model
{
    public class OAuthAccessToken
    {
        public string Access_Token { set; get; }
        public string Refresh_Token { set; get; }
        public string[] Scope { set; get; }
    }
}
