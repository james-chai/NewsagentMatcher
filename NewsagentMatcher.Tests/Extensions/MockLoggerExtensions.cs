using Microsoft.Extensions.Logging;
using Moq;

namespace NewsagentMatcher.Tests.Extensions;

public static class MockLoggerExtensions
{
    public static void VerifyLog<T>(
        this Mock<ILogger<T>> logger,
        LogLevel level,
        string? message = null,
        Times? times = null)
    {
        times ??= Times.Once();

        logger.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => message == null || o.ToString()!.Contains(message)),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            times.Value);
    }
}
