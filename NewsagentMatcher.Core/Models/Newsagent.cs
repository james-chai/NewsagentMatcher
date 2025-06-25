namespace NewsagentMatcher.Core.Models;

public record Newsagent
{
    public required string Name { get; init; }
    public required string Address1 { get; init; }
    public string? Address2 { get; init; }
    public required string City { get; init; }
    public required string State { get; init; }
    public required string PostCode { get; init; }
    public required double Latitude { get; init; }
    public required double Longitude { get; init; }
}
