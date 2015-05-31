using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch.Api.HttpServer
{
    public class StringEventArg
    {
        public string Text { set; get; }

        public StringEventArg(string text)
        {
            Text = text;
        }
    }
}
