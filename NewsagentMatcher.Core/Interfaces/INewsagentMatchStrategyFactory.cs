namespace NewsagentMatcher.Core.Interfaces;

public interface INewsagentMatchStrategyFactory
{
    INewsagentMatchStrategy GetStrategy(string chainId);
}
