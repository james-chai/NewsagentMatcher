using NewsagentMatcher.Core.Models;

namespace NewsagentMatcher.Core.Interfaces;

public interface INewsagentMatcher
{
    Task<List<(Newsagent agent, ValidationResult result)>> ValidateNewsagentsAsync(IEnumerable<ZineCoNewsagent> zineCoNewsagents,
                                                                                         IEnumerable<Newsagent> hewsagents,
                                                                                         CancellationToken cancellationToken = default);
}
