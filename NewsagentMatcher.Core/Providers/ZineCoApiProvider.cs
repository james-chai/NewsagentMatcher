using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NewsagentMatcher.Core.Configuration;
using NewsagentMatcher.Core.Interfaces;
using NewsagentMatcher.Core.Models;
using System.Text.Json;

namespace NewsagentMatcher.Core.Providers;

public class ZineCoApiProvider(
    HttpClient httpClient,
    IOptions<ZineCoSettings> settings,
    ILogger<ZineCoApiProvider> logger) : IZineCoDataProvider
{
    public async Task<IEnumerable<ZineCoNewsagent>> GetZineCoNewsagentsAsync(
        CancellationToken cancellationToken = default)
    {
        var endpoint = settings.Value.Endpoint;

        try
        {
            logger.LogInformation("Fetching ZineCo Newsagents from {Endpoint}", endpoint);

            var response = await httpClient.GetAsync(endpoint, cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            var result = JsonSerializer.Deserialize<List<ZineCoNewsagent>>(json) ?? [];

            logger.LogInformation("\nFetched {Count} newsagents from ZineCo:\n{Json}", result.Count, json);

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch ZineCo agents from {Endpoint}", endpoint);
            throw;
        }
    }
}
