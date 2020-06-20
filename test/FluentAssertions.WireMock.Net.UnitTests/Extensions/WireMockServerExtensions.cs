using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace FluentAssertions.WireMock.UnitTests.Extensions
{
    public static class WireMockServerExtensions
    {
        public static WireMockServer WithDefaultServer(this WireMockServer server)
        {
            server.Given(Request.Create().UsingAnyMethod())
                .RespondWith(Response.Create().WithStatusCode(200));

            return server;
        }
    }
}