namespace NewsagentMatcher.Core.Configuration;

public record ZineCoSettings
{
    public const string SectionName = "ZineCo";
    public string Endpoint { get; init; } = string.Empty;
    public int TimeoutSeconds { get; init; } = 30;
}
