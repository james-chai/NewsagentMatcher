namespace NewsagentMatcher.Core.Models;

public record ZineCoNewsagent : Newsagent
{
    public required string ChainId { get; init; }
}
