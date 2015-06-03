namespace Twitch.Api.Model.VerifyElements
{
    public class VerifyToken
    {
        public VerifyAuthorization Authorization {set; get; }
        public string User_Name { set; get; }
        public bool Valid { set; get; }
    }
}
