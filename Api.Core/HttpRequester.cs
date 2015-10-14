using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Api.Core
{
    public static class HttpRequester
    {
        public static async Task<string> Get(string baseUri, string parameterString)
        {
            var client = new HttpClient { BaseAddress = new Uri(baseUri) };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.GetAsync(parameterString);
            var message = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
                return message;

            throw new Exception(message);
        }

        public static async Task<bool> Put(string baseUri, string parameterString, FormUrlEncodedContent content)
        {
            var client = new HttpClient() { BaseAddress = new Uri(baseUri) };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.PutAsync(parameterString, content);
            
            if (response.StatusCode == HttpStatusCode.OK)
                return true;

            var message = await response.Content.ReadAsStringAsync();

            throw new Exception(message);
        }
    }
}
