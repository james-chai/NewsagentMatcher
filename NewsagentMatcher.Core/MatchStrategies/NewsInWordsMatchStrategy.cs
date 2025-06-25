using NewsagentMatcher.Core.Interfaces;
using NewsagentMatcher.Core.Models;

namespace NewsagentMatcher.Core.MatchStrategies;

public sealed class NewsInWordsMatchStrategy : INewsagentMatchStrategy
{
    private const string ChainIdentifier = "NIW";
    private const string StrategyDescription = "Reversed name match (News In Words)";

    public string ChainId => ChainIdentifier;
    public string Description => StrategyDescription;

    public bool IsMatch(Newsagent newsagent, ZineCoNewsagent zineCoNewsagent)
    {
        if (newsagent == null) throw new ArgumentNullException(nameof(newsagent));
        if (zineCoNewsagent == null) throw new ArgumentNullException(nameof(zineCoNewsagent));

        return HasReversedNameMatch(newsagent.Name, zineCoNewsagent.Name) &&
               HasMatchingAddress(newsagent, zineCoNewsagent);
    }

    private static bool HasReversedNameMatch(string newsagentName, string zineCoName)
        => ReverseWords(newsagentName).Equals(zineCoName, StringComparison.OrdinalIgnoreCase);

    private static bool HasMatchingAddress(Newsagent newsagent, ZineCoNewsagent zineCoNewsagent)
        => newsagent.Address1 == zineCoNewsagent.Address1 &&
           newsagent.City == zineCoNewsagent.City &&
           newsagent.State == zineCoNewsagent.State &&
           newsagent.PostCode == zineCoNewsagent.PostCode;

    private static string ReverseWords(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        Span<string> parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        parts.Reverse();

        return string.Join(' ', parts.ToArray());
    }
}