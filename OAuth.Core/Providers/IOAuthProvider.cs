using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OAuth.Core.Model;

namespace OAuth.Core.Providers
{
    public interface IOAuthProvider
    {
        string GetAuthUri();
        string GetTokenQueryUri();
        string GetAccessTokenUri(string authCode);
        string GetValidateTokenUri(string oAuthToken);
        string GetBaseUri();
        string GetSuccessUri();
        string ExtractAccessCode(string json);
        string ExtractErrorMessage(string json);
        TokenInfo ExtractVerifiedMessage(string json);
    }
}
