using Microsoft.Extensions.Logging;

namespace AutoRunManager;

public static class StaticFileLogger
{
    private static readonly string _filePath;
    private static readonly object _lock = new();

    static StaticFileLogger()
    {
        string logDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AutoRunManager");
        Directory.CreateDirectory(logDirectory);

        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        _filePath = Path.Combine(logDirectory, $"log_{timestamp}.txt");
    }

    public static void Log(LogLevel logLevel, string message)
    {
        if (logLevel == LogLevel.None) return;

        lock (_lock)
        {
            File.AppendAllText(_filePath, $"{DateTime.Now}: {logLevel} - {message}\n");
        }
    }

    public static void LogInformation(string message) => Log(LogLevel.Information, message);
    public static void LogError(string message) => Log(LogLevel.Error, message);
}