using NewsagentMatcher.Core.Models;

namespace NewsagentMatcher.Core.Interfaces;

public interface IZineCoDataProvider
{
    Task<IEnumerable<ZineCoNewsagent>> GetZineCoNewsagentsAsync(CancellationToken cancellationToken = default);
}