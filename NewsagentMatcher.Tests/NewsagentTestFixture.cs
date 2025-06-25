using Moq;
using NewsagentMatcher.Core.Interfaces;
using NewsagentMatcher.Core.Models;

namespace NewsagentMatcher.Tests;

public class NewsagentTestFixture : IDisposable
{
    public ZineCoNewsagent BaseAgent { get; }
    public Dictionary<string, ZineCoNewsagent> ChainTemplateAgents { get; }

    public Mock<INewsagentProvider> Provider { get; }

    public NewsagentTestFixture()
    {
        BaseAgent = new ZineCoNewsagent
        {
            ChainId = "SUP",
            Name = "Base Agent",
            Address1 = "123 Main St",
            City = "Testville",
            State = "TS",
            PostCode = "2000",
            Latitude = -33.865143,
            Longitude = 151.209900
        };

        // Chain-specific templates
        ChainTemplateAgents = new Dictionary<string, ZineCoNewsagent>
        {
            ["SUP"] = BaseAgent with
            {
                Name = "Super!! News!!",
                Address1 = "123 Main-Street;"
            },
            ["ADV"] = BaseAgent with
            {
                ChainId = "ADV",
                Name = "Adventure News",
                Latitude = -33.865143,
                Longitude = 151.209900
            },
            ["NIW"] = BaseAgent with
            {
                ChainId = "NIW",
                Name = "Words In News",
                Address1 = "456 Secondary Rd"
            }
        };

        Provider = new Mock<INewsagentProvider>();
    }

    public ZineCoNewsagent CreateAgentForChain(
        string chainId,
        Func<ZineCoNewsagent, ZineCoNewsagent>? transform = null)
    {
        var agent = ChainTemplateAgents[chainId];
        return transform?.Invoke(agent) ?? agent;
    }

    public void SetupProviderResponse(IEnumerable<ZineCoNewsagent> agents)
    {
        Provider.Reset();
        Provider.Setup(x => x.GetNewsagentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(agents.ToList());
    }

    public void Dispose()
    {
        // Cleanup if needed in future
    }
}
