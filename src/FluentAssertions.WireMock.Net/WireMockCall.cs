using FluentAssertions.Primitives;
using WireMock.Server;

namespace FluentAssertions.WireMock
{
    public class WireMockCall : ReferenceTypeAssertions<WireMockServer, WireMockCall>
    {
        public WireMockCall(WireMockServer server)
        {
            Subject = server;
        }
        
        public WireMockAssertions HaveBeenCalled()
        {
            return new WireMockAssertions(Subject);
        }

        protected override string Identifier => "wiremockserver";
    }
}