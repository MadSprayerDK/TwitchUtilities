using System;

namespace Twitch.Api.Model.VerifyElements
{
    public class VerifyAuthorization
    {
        public string[] Scopes { set; get; }
        public DateTime Created_At { set; get; }
        public DateTime Updated_At { set; get; }
    }
}
