using Nuki_Opener_Notifier.services;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var api = new NukiBridgeAPIService();
        await api.DiscoverBridgeUri();
    }
}