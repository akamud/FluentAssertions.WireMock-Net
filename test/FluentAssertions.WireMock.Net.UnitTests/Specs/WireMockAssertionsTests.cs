using FluentAssertions.WireMock.UnitTests.Extensions;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WireMock.Server;

namespace FluentAssertions.WireMock.UnitTests.Specs
{
    public class WireMockAssertionsTests
    {
        private WireMockServer _server;
        private HttpClient _httpClient;
        private const int Port = 42000;

        [SetUp]
        public void Setup()
        {
            _server = WireMockServer.Start(Port);
            _httpClient = new HttpClient {BaseAddress = new Uri($"http://localhost:{Port}")};
        }

        [Test]
        public void AtAbsoluteUrlShouldThrowWhenNoCallsWereMade()
        {
            _server.WithDefaultServer();

            Action act = () => _server.Should()
                .HaveReceivedACall()
                .AtAbsoluteUrl("anyurl");

            act.Should().Throw<Exception>()
                .And.Message.Should()
                .Be(
                    "Expected _server to have been called at address matching the absolute url \"anyurl\", but no calls were made.");
        }

        [Test]
        public async Task AtAbsoluteUrlShouldThrowWhenNoCallsMatchingTheAbsoluteUrlWereMade()
        {
            _server.WithDefaultServer();

            await _httpClient.GetAsync("");

            Action act = () => _server.Should()
                .HaveReceivedACall()
                .AtAbsoluteUrl("anyurl");

            act.Should().Throw<Exception>()
                .And.Message.Should()
                .Be(
                    $"Expected _server to have been called at address matching the absolute url \"anyurl\", but didn't find it among the calls to {{\"http://localhost:{Port}/\"}}.");
        }

        [TearDown]
        public void TearDown()
        {
            _server?.Stop();
            _server?.Dispose();
        }
    }
}