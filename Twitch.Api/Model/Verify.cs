using Twitch.Api.Model.Links;
using Twitch.Api.Model.VerifyElements;

namespace Twitch.Api.Model
{
    public class Verify
    {
        public VerifyToken Token { set; get; }
        public VerifyLinks _Links { set; get; }
    }
}
