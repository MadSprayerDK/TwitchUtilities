﻿namespace Twitch.Api.HttpServer
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
