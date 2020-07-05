using FluentAssertions.Equivalency;
using FluentAssertions.Execution;
using System;
using System.Linq;
using WireMock.Server;

namespace FluentAssertions.WireMock
{
    public class WireMockAssertions
    {
        private readonly WireMockServer _subject;
        private const string HeadersIdentifier = "headers";

        public WireMockAssertions(WireMockServer subject, int? callsCount)
        {
            _subject = subject;
        }

        [CustomAssertion]
        public AndConstraint<WireMockAssertions> AtAbsoluteUrl(string absoluteUrl, string because = "",
            params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .Given(() => _subject.LogEntries.Select(x => x.RequestMessage).ToList())
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

        // // TODO: faz diferença?
        // [CustomAssertion]
        // public AndConstraint<WireMockAssertions> WithHeader(string key, string value, string because = "",
        //     params object[] becauseArgs)
        // {
        //     return WithHeader(key, value, because, becauseArgs);
        // }

        // TODO: faz diferença?
        [CustomAssertion]
        public AndConstraint<WireMockAssertions> WithHeader(string key, string value,
            string because = "", params object[] becauseArgs)
        {
            var headersDictionary = _subject.LogEntries.SelectMany(x => x.RequestMessage.Headers)
                .ToDictionary(x => x.Key, x => x.Value);
            
            using (new AssertionScope("headers from requests sent"))
            {
                headersDictionary.Should().ContainKey(key, because, becauseArgs);
            }

            using (new AssertionScope($"header {{{key}}} from requests sent with value(s)"))
            {
                headersDictionary[key].Should().Contain(value);
            }

            return new AndConstraint<WireMockAssertions>(this);

            // POC
            // if (config is null)
            // {
            //     throw new ArgumentNullException(nameof(config));
            // }
            //
            // if (ReferenceEquals(_subject, null))
            // {
            //     Execute.Assertion
            //         .BecauseOf(because, becauseArgs)
            //         .WithDefaultIdentifier(HeadersIdentifier)
            //         .FailWith($"Expected {{context:{HeadersIdentifier}}} to contain of {{0}}{{reason}}, but found <null>.", key);
            // }
            //
            // EquivalencyAssertionOptions<string> options = config(AssertionOptions.CloneDefaults<string>());
            //
            // using (var scope = new AssertionScope())
            // {
            //     scope.AddReportable("configuration", options.ToString());
            //
            //     var headers = _subject.LogEntries.SelectMany(x => x.RequestMessage.Headers)
            //     foreach (object actualItem in )
            //     {
            //         var context = new EquivalencyValidationContext
            //         {
            //             Subject = actualItem,
            //             Expectation = expectation,
            //             CompileTimeType = typeof(TExpectation),
            //             Because = because,
            //             BecauseArgs = becauseArgs,
            //             Tracer = options.TraceWriter,
            //         };
            //
            //         var equivalencyValidator = new EquivalencyValidator(options);
            //         equivalencyValidator.AssertEquality(context);
            //
            //         string[] failures = scope.Discard();
            //
            //         if (!failures.Any())
            //         {
            //             return new AndConstraint<WireMockAssertions>(this);
            //         }
            //     }
            //
            //     Execute.Assertion
            //         .BecauseOf(because, becauseArgs)
            //         .FailWith("Expected {context:collection} {0} to contain equivalent of {1}{reason}.", Subject,
            //             expectation);
            // }
            //
            // return new AndConstraint<TAssertions>((TAssertions) this);
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
            //     Subject = _subject,
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
            ((object) _subject).Should().BeEquivalentTo(expectation, c, because, becauseArgs);

            return new AndConstraint<WireMockAssertions>(this);
        }
    }
}