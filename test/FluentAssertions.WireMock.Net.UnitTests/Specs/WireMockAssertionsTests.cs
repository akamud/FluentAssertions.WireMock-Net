using FluentAssertions.Formatting;
using FluentAssertions.WireMock.UnitTests.Extensions;
using NUnit.Framework;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
        
        [Test]
        public async Task WithHeaderShouldThrowWhenNoCallsMatchingTheAHeaderWereMade()
        {
            _server.WithDefaultServer();

            await _httpClient.GetAsync("");
            
            // _httpClient
            //     .DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            // _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //     await _httpClient.GetAsync("");

            Action act = () => _server.Should()
                .HaveReceivedACall()
                .WithHeader("Authorization", "value");

            var sentHeaders = _server.LogEntries.SelectMany(x => x.RequestMessage.Headers)
                .ToDictionary(x => x.Key, x => x.Value)
                .ToList();

            var sentHeaderString = "{" + string.Join(", ", sentHeaders) + "}";
            
            act.Should().Throw<Exception>()
                .And.Message.Should()
                .Be(
                    $"Expected headers from requests sent {sentHeaderString} to contain key \"Authorization\".\n");
        }

        [TearDown]
        public void TearDown()
        {
            _server?.Stop();
            _server?.Dispose();
        }
    }
}