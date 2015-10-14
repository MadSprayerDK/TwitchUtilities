using System.Collections.Generic;
using Api.Twitch.Model.SearchGamesElements;

namespace Api.Twitch.Model
{
    public class SearchGames
    {
        public global::Api.Twitch.Model.Links.SearchGames _Links { set; get; }
        public List<SearchGamesGame> Games { set; get; } 
    }
}
