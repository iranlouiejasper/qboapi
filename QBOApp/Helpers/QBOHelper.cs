using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace QBOApp.Helpers
{
    public static class QBOHelper
    {
        public static string GetAuthURL(string authUrl,string clientId,string scope,string redirectUrl,string state)
        {
            var builder = new UriBuilder(authUrl);
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["client_id"] = clientId;
            query["response_type"] = "code";
            query["scope"] = scope;
            query["redirect_uri"] = redirectUrl;
            query["state"] = state;

            builder.Query = query.ToString();
            string url = builder.ToString();

            return url;
        }

        public static async Task<TokenResponse> GetAccessTokenByCode(string tokenUrl, string code,string clientId,string clientSecret,string redirectUrl)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var stringBytes = Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}");
            var encodedBytes = Convert.ToBase64String(stringBytes);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, tokenUrl);

            request.Headers.TryAddWithoutValidation("Accept", "application/json");
            request.Headers.TryAddWithoutValidation("Authorization", $"Basic {encodedBytes}");

            var contentList = new List<string>();
            contentList.Add("grant_type=authorization_code");
            contentList.Add($"code={code}");
            contentList.Add($"redirect_uri={redirectUrl}");
            request.Content = new StringContent(string.Join("&", contentList));
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<TokenResponse>(await response.Content.ReadAsStringAsync());
            }

            return null;
        }

        public static async Task<TokenResponse> RefreshAccessToken(string tokenUrl, string refreshToken, string clientId, string clientSecret)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var stringBytes = Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}");
            var encodedBytes = Convert.ToBase64String(stringBytes);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, tokenUrl);

            request.Headers.TryAddWithoutValidation("Accept", "application/json");
            request.Headers.TryAddWithoutValidation("Authorization", $"Basic {encodedBytes}");

            var contentList = new List<string>();
            contentList.Add("grant_type=refresh_token");
            contentList.Add($"refresh_token={refreshToken}");
            request.Content = new StringContent(string.Join("&", contentList));
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<TokenResponse>(await response.Content.ReadAsStringAsync());
            }

            return null;
        }

        public static async Task<object> QueryEntities(string accessToken, string baseUrl, string realmId , string query)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string url = $"{baseUrl}/v3/company/{realmId}/query?query={query}";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.TryAddWithoutValidation("Accept", "application/json");
            request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {accessToken}");

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<object>(await response.Content.ReadAsStringAsync());
            }

            return null;
        }
    }

    public class TokenResponse
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string expires_in { get; set; }
        public string x_refresh_token_expires_in { get; set; }
        public string token_type { get; set; }
    }
}