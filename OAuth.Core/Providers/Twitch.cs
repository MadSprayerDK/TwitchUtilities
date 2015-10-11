using System;
using Newtonsoft.Json;
using OAuth.Core.Model;

namespace OAuth.Core.Providers
{
    public class Twitch : IOAuthProvider
    {
        private readonly string[] _scopes;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _redirectUri;
        private readonly string _successUri;
        private string _apiBaseUri = "https://api.twitch.tv/kraken/";

        public Twitch(string[] scopes, string clientId, string clientSecret, string redirectUri, string successUri)
        {
            _scopes = scopes;
            _clientId = clientId;
            _clientSecret = clientSecret;
            _redirectUri = redirectUri;
            _successUri = successUri;
        }

        public string GetAuthUri()
        {
            var scopes = string.Join("+", _scopes);
            var uri = _apiBaseUri + "oauth2/authorize" +
                      "?response_type=code" +
                      "&client_id=" + _clientId +
                      "&redirect_uri=" + _redirectUri +
                      "&scope=" + scopes;

            return uri;
        }

        public string GetTokenQueryUri()
        {
            return "oauth2/token";
        }

        public string GetAccessTokenUri(string authCode)
        {
            var uri = "?client_id=" + _clientId +
                      "&client_secret=" + _clientSecret +
                      "&grant_type=authorization_code" +
                      "&redirect_uri=" + _redirectUri +
                      "&code=" + authCode;

            return uri;
        }

        public string GetValidateTokenUri(string oAuthToken)
        {
            return "?oauth_token=" + oAuthToken;
        }

        public string GetBaseUri()
        {
            return _apiBaseUri;
        }

        public string GetSuccessUri()
        {
            return _successUri;
        }

        public string ExtractAccessCode(string json)
        {
            var token = JsonConvert.DeserializeObject<TwitchAccessCode>(json);
            return token.Access_Token;
        }

        public string ExtractErrorMessage(string json)
        {
            var error = JsonConvert.DeserializeObject<TwitchErrorQuery>(json);
            return error.Error + ": " + error.Message;
        }

        public TokenInfo ExtractVerifiedMessage(string json)
        {
            var info = new TokenInfo();
            var verifiedToken = JsonConvert.DeserializeObject<TwitchVerify>(json);

            info.Valid = verifiedToken.Token.Valid;

            if (!info.Valid)
                return info;

            info.UserName = verifiedToken.Token.User_Name;
            info.Scope = verifiedToken.Token.Authorization.Scopes;

            return info;
        }
    }

    internal class TwitchAccessCode
    {
        public string Access_Token { set; get; }
        public string Refresh_Token { set; get; }
        public string[] Scope { set; get; }
    }

    internal class TwitchErrorQuery
    {
        public string Error { set; get; }
        public int Status { set; get; }
        public string Message { set; get; }
    }

    // Verification Classes
    internal class TwitchVerify
    {
        public TwitchVerifyToken Token { set; get; }
        public TwitchVerifyLinks _Links { set; get; }
    }

    internal class TwitchVerifyToken
    {
        public TwitchVerifyAuthorization Authorization { set; get; }
        public string User_Name { set; get; }
        public bool Valid { set; get; }
    }

    internal class TwitchVerifyAuthorization
    {
        public string[] Scopes { set; get; }
        public DateTime Created_At { set; get; }
        public DateTime Updated_At { set; get; }
    }

    internal class TwitchVerifyLinks
    {
        public string Channel { set; get; }
        public string Users { set; get; }
        public string User { set; get; }
        public string Channels { set; get; }
        public string Chat { set; get; }
        public string Streams { set; get; }
        public string Ingests { set; get; }
        public string Teams { set; get; }
        public string Search { set; get; }
    }
}