using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NewsagentMatcher.Core.Configuration;
using NewsagentMatcher.Core.Interfaces;
using NewsagentMatcher.Core.Providers;

namespace NewsagentMatcher.Core.Factories;

public class NewsagentProviderFactory(
    IHttpClientFactory httpClientFactory,
    IOptions<NewsagentChainSettings> settings,
    ILogger<NewsagentApiProvider> logger)
{
    public INewsagentProvider Create(string chainId)
        => new NewsagentApiProvider(httpClientFactory.CreateClient("NewsagentClient"), settings, logger, chainId);
}