using NewsagentMatcher.Core.Models;

namespace NewsagentMatcher.Core.Interfaces;

public interface INewsagentProvider
{
    Task<IEnumerable<Newsagent>> GetNewsagentsAsync(CancellationToken cancellationToken = default);
}
