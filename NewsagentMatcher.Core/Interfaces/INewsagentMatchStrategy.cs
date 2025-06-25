using NewsagentMatcher.Core.Models;

namespace NewsagentMatcher.Core.Interfaces;

public interface INewsagentMatchStrategy
{
    string Description { get; }
    string ChainId { get; }
    bool IsMatch(Newsagent newsagent, ZineCoNewsagent zineCoNewsagent);
}
