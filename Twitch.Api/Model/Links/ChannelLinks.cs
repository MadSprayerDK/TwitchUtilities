using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch.Api.Model.Link
{
    public class ChannelLinks
    {
        public string Self { set; get; }
        public string Follows { set; get; }
        public string Commercial { set; get; }
        public string Stream_Key { set; get; }
        public string Chat { set; get; }
        public string Features { set; get; }
        public string Subscriptions { set; get; }
        public string Editors { set; get; }
        public string Teams { set; get; }
        public string Videos { set; get; }
    }
}
