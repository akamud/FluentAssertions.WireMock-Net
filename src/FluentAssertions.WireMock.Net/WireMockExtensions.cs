using WireMock.Server;

namespace FluentAssertions.WireMock
{
    public static class WireMockExtensions
    {
        public static WireMockCall Should(this WireMockServer instance)
        {
            return new WireMockCall(instance);
        }
    }
}