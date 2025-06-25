using NewsagentMatcher.Core.MatchStrategies;
using NewsagentMatcher.Core.Models;

namespace NewsagentMatcher.Tests;

using Xunit;

public class SuperNewsMatchStrategyTests
{
    private readonly SuperNewsMatchStrategy _strategy = new();

    [Theory]
    [InlineData("Circular(Quay'News", "Circular Quay News", "123 Main-Street;", "123 Main Street", true)]
    [InlineData("Super!! News!!", "Super News", "123 Main-Street;", "123 Main Street", true)]
    [InlineData("Super News", "Different News", "123 Main Street", "123 Main Street", false)]
    [InlineData("Super@News", "Super News", "124 Main Street", "123 Main Street", false)]
    [InlineData("SUPER news!", "super News", "123 Main Street", "123 main street", true)]
    [InlineData("Super!!News", "Super News", "123 Main Street", "123 Main Street", true)]
    public void IsMatch_ReturnsTrue_WhenNameAndAddressMatchAfterPunctuationRemoved_Cases(
        string newsagentName,
        string zineCoName,
        string newsagentAddress1,
        string zineCoAddress1,
        bool expected)
    {
        // Arrange
        var newsagent = new Newsagent
        {
            Name = newsagentName,
            Address1 = newsagentAddress1,
            Address2 = null,
            City = "Testville",
            State = "TS",
            PostCode = "2000",
            Latitude = 0D,
            Longitude = 0D
        };

        var zineCoNewsagent = new ZineCoNewsagent
        {
            ChainId = "SUP",
            Name = zineCoName,
            Address1 = zineCoAddress1,
            Address2 = null,
            City = "Testville",
            State = "TS",
            PostCode = "2000",
            Latitude = newsagent.Latitude,
            Longitude = newsagent.Longitude
        };

        // Act
        var result = _strategy.IsMatch(newsagent, zineCoNewsagent);

        // Assert
        Assert.Equal(expected, result);
    }
}

public class SuperNewsMatcherTests : IClassFixture<NewsagentTestFixture>
{
    private readonly NewsagentTestFixture _fixture;

    public SuperNewsMatcherTests(NewsagentTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void IsMatch_ReturnsTrue_WhenNameAndAddressMatchAfterPunctuationRemoved()
    {
        // Arrange
        var strategy = new SuperNewsMatchStrategy();

        var zineCoAgent = new ZineCoNewsagent
        {
            ChainId = "SUP",
            Name = "Super News",
            Address1 = "123 Main Street",
            City = "Testville",
            State = "TS",
            PostCode = "2000",
            Latitude = -33.865143,
            Longitude = 151.209900
        };

        var chainAgent = new Newsagent
        {
            Name = "Super!! News!!",
            Address1 = "123 Main-Street;",
            Address2 = null,
            City = "Testville",
            State = "TS",
            PostCode = "2000",
            Latitude = -33.865143,
            Longitude = 151.209900
        };

        // Act
        var result = strategy.IsMatch(chainAgent, zineCoAgent);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsMatch_ReturnsFalse_WhenCityIsDifferent()
    {
        var strategy = new SuperNewsMatchStrategy();

        var zineCoAgent = _fixture.CreateAgentForChain("SUP");
        var chainAgent = new Newsagent
        {
            Name = "Super!! News!!",
            Address1 = "123 Main-Street;",
            Address2 = null,
            City = "OtherCity", // City differs
            State = "TS",
            PostCode = "2000",
            Latitude = -33.865143,
            Longitude = 151.209900
        };

        var result = strategy.IsMatch(chainAgent, zineCoAgent);

        Assert.False(result);
    }
}
