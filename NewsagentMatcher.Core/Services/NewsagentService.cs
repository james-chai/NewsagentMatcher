using Microsoft.Extensions.Logging;
using NewsagentMatcher.Core.Interfaces;
using NewsagentMatcher.Core.Models;

namespace NewsagentMatcher.Core.Services;

public class NewsagentService(
    INewsagentMatchStrategyFactory strategyFactory,
    Func<string, INewsagentProvider> providerFactory,
    IZineCoDataProvider zineCoProvider) : INewsagentService
{
    public async Task<List<(Newsagent, ValidationResult)>> ValidateByChainAsync(
        string chainId,
        IEnumerable<ZineCoNewsagent> zineCoAgents,
        IEnumerable<Newsagent> chainAgents,
        CancellationToken cancellationToken)
    {
        var strategy = strategyFactory.GetStrategy(chainId);
        var matcher = new GenericNewsagentMatcher(strategy);

        return await matcher.ValidateNewsagentsAsync(zineCoAgents, chainAgents, cancellationToken);
    }

    public async Task<IEnumerable<ZineCoNewsagent>> GetZineCoNewsagentsAsync(CancellationToken cancellationToken = default)
    {
        return await zineCoProvider.GetZineCoNewsagentsAsync(cancellationToken);
    }

    public async Task<IEnumerable<Newsagent>> GetNewsagentsAsync(string chainId, CancellationToken cancellationToken = default)
    {
        var provider = providerFactory(chainId);
        return await provider.GetNewsagentsAsync(cancellationToken);
    }
}
