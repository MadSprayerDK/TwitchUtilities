﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch.Api.Model
{
    public class QueryError
    {
        public string Error { set; get; }
        public int Status { set; get; }
        public string Message { set; get; }
    }
}
