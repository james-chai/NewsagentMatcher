using NewsagentMatcher.Core.Interfaces;
using NewsagentMatcher.Core.Models;
using System.Text;

namespace NewsagentMatcher.Core.MatchStrategies;

public sealed class SuperNewsMatchStrategy : INewsagentMatchStrategy
{
    private const string ChainIdentifier = "SUP";
    private const string StrategyDescription = "Normalized field match (SuperNews)";

    public string ChainId => ChainIdentifier;
    public string Description => StrategyDescription;

    public bool IsMatch(Newsagent newsagent, ZineCoNewsagent zineCoNewsagent)
    {
        if (newsagent == null) throw new ArgumentNullException(nameof(newsagent));
        if (zineCoNewsagent == null) throw new ArgumentNullException(nameof(zineCoNewsagent));

        return AllFieldsMatch(newsagent, zineCoNewsagent);
    }

    private static bool AllFieldsMatch(Newsagent newsagent, ZineCoNewsagent zineCoNewsagent)
    {
        return FieldMatches(newsagent.Name, zineCoNewsagent.Name) &&
               FieldMatches(newsagent.Address1, zineCoNewsagent.Address1) &&
               FieldMatches(newsagent.Address2 ?? "", zineCoNewsagent.Address2 ?? "") &&
               FieldMatches(newsagent.City, zineCoNewsagent.City) &&
               FieldMatches(newsagent.State, zineCoNewsagent.State) &&
               FieldMatches(newsagent.PostCode, zineCoNewsagent.PostCode);
    }

    private static bool FieldMatches(string newsagentField, string zineCoField)
    {
        string normalizedNewsagent = Normalize(newsagentField);

        string cleanedZineCo = Clean(zineCoField);

        return normalizedNewsagent == cleanedZineCo;
    }

    private static string Normalize(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var normalized = new StringBuilder();
        foreach (char c in input.ToLowerInvariant())
        {
            normalized.Append(char.IsPunctuation(c) ? ' ' : c);
        }

        string result = string.Join(" ",
            normalized.ToString()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim()))
            .Trim();

        if (string.IsNullOrEmpty(result))
            return string.Empty;

        return result;
    }

    private static string Clean(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        return string.Join(" ",
            input.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                 .Select(s => s.Trim()))
            .ToLowerInvariant();
    }
}