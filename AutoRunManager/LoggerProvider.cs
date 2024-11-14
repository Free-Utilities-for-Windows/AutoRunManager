using Microsoft.Extensions.Logging;

namespace AutoRunManager;

public class LoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new FileLogger();

    public void Dispose() { }
}