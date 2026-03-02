using System.Text.RegularExpressions;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Semver;

namespace SurrealDb.Net.Tests.Extensions;

public static class StringAssertionsExtensions
{
    public static AndConstraint<StringAssertions> BeNanoid(this StringAssertions assertions)
    {
        const string nanoidPattern = "[a-z0-9]{20}";

        assertions.Subject.Should().NotBeNull();
        assertions.Subject.Should().MatchRegex(nanoidPattern);

        return new AndConstraint<StringAssertions>(assertions);
    }

    public static AndConstraint<StringAssertions> BeUlid(this StringAssertions assertions)
    {
        assertions.Subject.Should().NotBeNull();
        assertions.Subject.Should().HaveLength(26);

        return new AndConstraint<StringAssertions>(assertions);
    }

    public static AndConstraint<StringAssertions> BeUuid(this StringAssertions assertions)
    {
        assertions.Subject.Should().NotBeNull();
        Guid.TryParse(assertions.Subject, out _).Should().BeTrue();

        return new AndConstraint<StringAssertions>(assertions);
    }

    public static AndConstraint<StringAssertions> BeValidJwt(this StringAssertions assertions)
    {
        // TODO : Use System.IdentityModel.Tokens.Jwt library to check if the JWT is valid?
        const string jwtRegexPattern = @"^[A-Za-z0-9-_=]+\.[A-Za-z0-9-_=]+\.?[A-Za-z0-9-_.+/=]*$";

        assertions.Subject.Should().NotBeNullOrWhiteSpace();
        Regex.IsMatch(assertions.Subject!, jwtRegexPattern).Should().BeTrue();

        return new AndConstraint<StringAssertions>(assertions);
    }

    public static AndConstraint<StringAssertions> BeValidSemver(
        this StringAssertions assertions,
        string? prefix = null,
        string? suffix = null
    )
    {
        string escapedPrefix = Regex.Escape(prefix ?? string.Empty);
        string escapedSuffix = Regex.Escape(suffix ?? string.Empty);
        string regexPattern = $"^{escapedPrefix}(.*){escapedSuffix}$";

        var match = Regex.Match(assertions.Subject, regexPattern);

        if (!match.Success)
        {
            assertions.Subject.Should().MatchRegex(regexPattern);

            return new AndConstraint<StringAssertions>(assertions);
        }

        var semverCandidate = match.Groups[1].Value;

        assertions.Subject.Should().NotBeNull();
        SemVersion.TryParse(semverCandidate, SemVersionStyles.Strict, out _).Should().BeTrue();

        return new AndConstraint<StringAssertions>(assertions);
    }
}
