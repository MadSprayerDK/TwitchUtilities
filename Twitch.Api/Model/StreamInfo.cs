using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twitch.Api.Model.Link;
using Twitch.Api.Model.StreamElements;

namespace Twitch.Api.Model
{
    public class StreamInfo
    {
        public StreamLinks _Links { set; get; }
        public StreamStream Stream { set; get; }
    }
}
