namespace Api.Twitch.Model.SearchGamesElements
{
    public class SearchGamesGame
    {
        public SearchGamesImages Box { set; get; }
        public SearchGamesImages Logo { set; get; }
        public int Popularity { set; get; }
        public string Name { set; get; }
        public int _Id { set; get; }
        public int Giantbomb_Id { set; get; }
    }
}
