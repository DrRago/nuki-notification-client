using Nuki_Opener_Notifier.models;
using System.Text.Json;

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
    }
}
