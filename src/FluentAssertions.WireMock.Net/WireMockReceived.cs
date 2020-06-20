using FluentAssertions.Primitives;
using WireMock.Server;

namespace FluentAssertions.WireMock
{
    public class WireMockReceived : ReferenceTypeAssertions<WireMockServer, WireMockReceived>
    {
        public WireMockReceived(WireMockServer server)
        {
            Subject = server;
        }
        
        public WireMockAssertions HaveReceivedACall()
        {
            return new WireMockAssertions(Subject, null);
        }
        
        public WireMockANumberOfCalls HaveReceived(int callsCount)
        {
            return new WireMockANumberOfCalls(Subject, callsCount);
        }

        protected override string Identifier => "wiremockserver";
    }
}