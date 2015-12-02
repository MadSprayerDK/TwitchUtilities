using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Spotify.Responses
{
    public class Resource
    {
        public string Name { set; get; }
        public string Uri { set; get; }
        public Location Location { set; get; } 
    }
}
