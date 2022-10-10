using Nuki_Opener_Notifier.models;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text.Json;
using System.Web;
using Windows.UI.Composition;

namespace Nuki_Opener_Notifier.services
{
    internal class NukiBridgeAPIService
    {
        /// <summary>
        /// Local bridgeUri for api communications
        /// </summary>
        private string bridgeUri;

        /// <summary>
        /// Nuki bridge api discovery endpoint. Lists all bridges comming from the requesters public IP
        /// </summary>
        private readonly string nukiBridgeDiscoveryUri = "https://api.nuki.io/discover/bridges";

        private int authToken;

        public async Task DiscoverBridgeUri()
        {
            HttpResponseMessage response = await new HttpClient().GetAsync(nukiBridgeDiscoveryUri);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                BridgeDiscoveyModel? responseObject = JsonSerializer.Deserialize<BridgeDiscoveyModel>(responseString);

                if (responseObject != null && responseObject.errorCode == 0)
                {
                    // TODO make some kind of configuration possible if more than one bridge is in the network
                    if (responseObject.bridges.Count == 1)
                    {
                        var bridge = responseObject.bridges[0];
                        bridgeUri = $"http://{bridge.ip}:{bridge.port}/";
                    }
                    else
                    {
                        throw new NullReferenceException("No Nuki Bridge found from your IP. Is the API enabled?");
                    }
                }
                else
                {
                    throw new JsonException("Could not parse JSON answer from Nuki server");
                }
            }
            else
            {
                throw new HttpRequestException($"Request to '{response.RequestMessage?.RequestUri}' failed with status code {response.StatusCode}");
            }
        }

        private async Task<HttpResponseMessage> PerformRequest(string endpoint, NameValueCollection queryParams)
        {
            if (authToken == 0)
            {
                int token = 0;
                do
                {
                    if (token != 0)
                    {
                        MessageBox.Show("Authorization token invalid.", "Auth token validation error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    token = DataService.GetAuthorizationToken();
                } while (!await ValidateToken(token));
            }
            queryParams["token"] = authToken.ToString();
            var builder = new UriBuilder(bridgeUri)
            {
                Query = queryParams.ToString(),
                Path = endpoint
            };
            string url = builder.ToString();
            return await new HttpClient().GetAsync(url);

        }

        private async Task<HttpResponseMessage> PerformRequest(string endpoint)
        {
            return await PerformRequest(endpoint, HttpUtility.ParseQueryString(string.Empty));
        }

        public async Task<bool> ValidateToken(int token)
        {
            authToken = token;

            var response = await PerformRequest("list");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                authToken = 0;
                return false;
            }

            return true;
        }
    }
}
