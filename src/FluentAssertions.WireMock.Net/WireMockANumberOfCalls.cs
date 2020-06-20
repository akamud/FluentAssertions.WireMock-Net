using WireMock.Server;

namespace FluentAssertions.WireMock
{
    public class WireMockANumberOfCalls
    {
        private readonly WireMockServer _server;
        private readonly int _callsCount;

        public WireMockANumberOfCalls(WireMockServer server, int callsCount)
        {
            _server = server;
            _callsCount = callsCount;
        }

        public WireMockAssertions Calls()
        {
            return new WireMockAssertions(_server, _callsCount);
        }
    }
}