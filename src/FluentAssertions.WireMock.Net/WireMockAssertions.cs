using FluentAssertions.Equivalency;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using System;
using System.Linq;
using WireMock.Server;

namespace FluentAssertions.WireMock
{
    public class WireMockAssertions
    {
        private readonly WireMockServer _instance;

        public WireMockAssertions(WireMockServer instance, int? callsCount)
        {
            _instance = instance;
        }

        [CustomAssertion]
        public AndConstraint<WireMockAssertions> AtAbsoluteUrl(string absoluteUrl, string because = "",
            params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .Given(() => _instance.LogEntries.Select(x => x.RequestMessage).ToList())
                .ForCondition(requests => requests.Any())
                .FailWith(
                    "Expected {context:wiremockserver} to have been called at address matching the absolute url {0}{reason}, but no calls were made.",
                    absoluteUrl)
                .Then
                .ForCondition(x => x.Any(y => y.AbsoluteUrl == absoluteUrl))
                .FailWith(
                    "Expected {context:wiremockserver} to have been called at address matching the absolute url {0}{reason}, but didn't find it among the calls to {1}.",
                    _ => absoluteUrl, requests => requests.Select(request => request.AbsoluteUrl));

            return new AndConstraint<WireMockAssertions>(this);
        }

        [CustomAssertion]
        public AndConstraint<WireMockAssertions> WithHeader(string key, string value, string because = "",
            params object[] becauseArgs)
        {
            using (new AssertionScope("headers from requests sent"))
            {
                // TODO: Pegar todos valores da lista do WireMockList
                _instance.LogEntries.SelectMany(x => x.RequestMessage.Headers)
                    .ToDictionary(x => x.Key, x => x.Value.FirstOrDefault())
                    .Should().Contain(key, value, because, becauseArgs);
            }

            return new AndConstraint<WireMockAssertions>(this);
        }

        [CustomAssertion]
        public AndConstraint<WireMockAssertions> WithBodyEquivalentTo<TExpectation>(TExpectation expectation,
            Func<EquivalencyAssertionOptions<TExpectation>, EquivalencyAssertionOptions<TExpectation>> config,
            string because = "",
            params object[] becauseArgs)
        {
            // // Guard.ThrowIfArgumentIsNull(config, nameof(config));
            //
            // // EquivalencyAssertionOptions<TExpectation> options = config(AssertionOptions.CloneDefaults<TExpectation>());
            //
            // var context = new EquivalencyValidationContext
            // {
            //     Subject = _instance,
            //     Expectation = expectation,
            //     CompileTimeType = typeof(TExpectation),
            //     Because = because,
            //     BecauseArgs = becauseArgs,
            //     // Tracer = options.TraceWriter
            // };
            //
            // var equivalencyValidator = new EquivalencyValidator(AssertionOptions.CloneDefaults<TExpectation>());
            // equivalencyValidator.AssertEquality(context);

            Func<EquivalencyAssertionOptions<TExpectation>, EquivalencyAssertionOptions<TExpectation>> c = s =>
                AssertionOptions.CloneDefaults<TExpectation>();
            ((object) _instance).Should().BeEquivalentTo(expectation, c, because, becauseArgs);

            return new AndConstraint<WireMockAssertions>(this);
        }
    }
}