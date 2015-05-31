using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch.Api.Model
{
    public class OAuthAccessToken
    {
        public string Access_Token { set; get; }
        public string Refresh_Token { set; get; }
        public string[] Scope { set; get; }
    }
}
