namespace Nuki_Opener_Notifier.models
{
    public class Bridge
    {
        public int bridgeId { get; set; }
        public string ip { get; set; }
        public int port { get; set; }
        public DateTime dateUpdated { get; set; }
    }

    public class BridgeDiscoveyModel
    {
        public List<Bridge> bridges { get; set; }
        public int errorCode { get; set; }
    }

}
