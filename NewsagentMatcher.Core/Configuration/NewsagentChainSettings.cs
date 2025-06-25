namespace NewsagentMatcher.Core.Configuration;

public class NewsagentChainSettings
{
    public const string SectionName = "NewsagentChains";

    public Dictionary<string, ChainConfig> Chains { get; set; } = new();

    public class ChainConfig
    {
        public string Endpoint { get; init; } = string.Empty;
        public int TimeoutSeconds { get; init; } = 30;
    }
}
