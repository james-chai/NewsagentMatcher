using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NewsagentMatcher.Core.Configuration;
using NewsagentMatcher.Core.Interfaces;
using NewsagentMatcher.Core.Models;
using System.Net;
using System.Text.Json;

namespace NewsagentMatcher.Core.Providers;

public class NewsagentApiProvider(
    HttpClient httpClient,
    IOptions<NewsagentChainSettings> settings,
    ILogger<NewsagentApiProvider> logger,
    string chainId) : INewsagentProvider
{
    public async Task<IEnumerable<Newsagent>> GetNewsagentsAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var chain = settings.Value.Chains[chainId];

            logger.LogInformation("Fetching Newsagents from {Endpoint}", chain.Endpoint);
            var response = await httpClient.GetAsync(chain.Endpoint, cancellationToken);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            var result = JsonSerializer.Deserialize<List<Newsagent>>(json) ?? [];

            logger.LogInformation("\nFetched {Count} newsagents from {ChainId}\n{Json}", result.Count, chainId, json);

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve newsagents from {ChainId}", chainId);

            throw;
        }
    }
}
