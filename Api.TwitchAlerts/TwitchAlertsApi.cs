using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Core;
using Api.TwitchAlerts.Model;
using Newtonsoft.Json;

namespace Api.TwitchAlerts
{
    public class TwitchAlertsApi
    {
        private readonly string _apiBaseUri = "https://www.twitchalerts.com/api/v1.0/";
        private readonly string _oAuthToken;

        public TwitchAlertsApi(string oAuthToken)
        {
            _oAuthToken = oAuthToken;
        }

        public async Task<int> GetLastDonationId()
        {
            var data = JsonConvert.DeserializeObject<DonationDataStructure>(await HttpRequester.Get(_apiBaseUri, "donations?limit=1&access_token=" + _oAuthToken));
            var donation = data.Data.FirstOrDefault();

            return donation == null ? 0 : donation.Donation_Id;
        }

        public async Task<IList<Donation>> GetLastDonations(int after)
        {
            var data = JsonConvert.DeserializeObject<DonationDataStructure>(await HttpRequester.Get(_apiBaseUri, "donations?limit=100&access_token=" + _oAuthToken + "&after=" + after));
            return data.Data;
        }
    }
}
