using Nuki_Opener_Notifier.services;

namespace Nuki_Opener_Notifier
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main()
        {
            var api = new NukiBridgeAPIService();
            await api.DiscoverBridgeUri();
        }
    }
}