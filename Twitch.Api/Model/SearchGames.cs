using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twitch.Api.Model.SearchGamesElements;

namespace Twitch.Api.Model
{
    public class SearchGames
    {
        public Links.SearchGames _Links { set; get; }
        public List<SearchGamesGame> Games { set; get; } 
    }
}
