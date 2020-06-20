using WireMock.Server;

namespace FluentAssertions.WireMock
{
    public static class WireMockExtensions
    {
        public static WireMockReceived Should(this WireMockServer instance)
        {
            return new WireMockReceived(instance);
        }
    }
}