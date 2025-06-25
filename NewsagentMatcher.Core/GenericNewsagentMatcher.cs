using NewsagentMatcher.Core.Interfaces;
using NewsagentMatcher.Core.Models;
using System.Text.Json;

namespace NewsagentMatcher.Core;

public class GenericNewsagentMatcher(INewsagentMatchStrategy strategy)
    : INewsagentMatcher
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<List<(Newsagent agent, ValidationResult result)>> ValidateNewsagentsAsync(
        IEnumerable<ZineCoNewsagent> zineCoNewsagents,
        IEnumerable<Newsagent> newsagents,
        CancellationToken cancellationToken = default)
    {
        var results = new List<(Newsagent, ValidationResult)>();

        foreach (var newsagent in newsagents)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await Task.Run(() =>
            {
                var matchedNewsagent = zineCoNewsagents.FirstOrDefault(z => strategy.IsMatch(newsagent, z));

                var matchedJson = matchedNewsagent != null
                    ? JsonSerializer.Serialize(matchedNewsagent, JsonOptions)
                    : string.Empty;

                var newsagentJson = JsonSerializer.Serialize(newsagent, JsonOptions);

                if (matchedNewsagent != null)
                {
                    var message = $"✅ Match found for agent {newsagent.Name}\nZineCoNewsagent:\n{matchedJson}\nNewsAgent:\n{newsagentJson}";
                    return (newsagent, new ValidationResult(true, message));
                }
                else
                {
                    var message = $"❌ No match for agent {newsagent.Name}\nNewsagent:\n{newsagentJson}";
                    return (newsagent, new ValidationResult(false, message));
                }
            }, cancellationToken);

            results.Add(result);
        }

        return results;
    }
}
