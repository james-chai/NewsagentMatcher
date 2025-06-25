using NewsagentMatcher.Core.Interfaces;

namespace NewsagentMatcher.Core.Factories;

public class NewsagentMatchStrategyFactory : INewsagentMatchStrategyFactory
{
    private readonly Dictionary<string, INewsagentMatchStrategy> _strategies;

    public NewsagentMatchStrategyFactory(IEnumerable<INewsagentMatchStrategy> strategies)
        => _strategies = strategies.ToDictionary(s => s.ChainId.ToUpperInvariant());

    public INewsagentMatchStrategy GetStrategy(string chainId)
    {
        if (_strategies.TryGetValue(chainId.ToUpperInvariant(), out var strategy))
            return strategy;

        throw new ArgumentException($"Unknown chainId: {chainId}");
    }
}