using NewsagentMatcher.Core.Models;

namespace NewsagentMatcher.Core.Services
{
    public interface INewsagentService
    {
        Task<IEnumerable<Newsagent>> GetNewsagentsAsync(string chainId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ZineCoNewsagent>> GetZineCoNewsagentsAsync(CancellationToken cancellationToken = default);
        Task<List<(Newsagent, ValidationResult)>> ValidateByChainAsync(string chainId, IEnumerable<ZineCoNewsagent> zineCoAgents, IEnumerable<Newsagent> chainAgents, CancellationToken cancellationToken);
    }
}