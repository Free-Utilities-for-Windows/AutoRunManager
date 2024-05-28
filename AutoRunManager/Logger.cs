namespace AutoRunManager;

public class Logger
{
    private static string logFilePath = "log.txt";

    public static void Log(string message)
    {
        using (StreamWriter writer = new StreamWriter(logFilePath, true))
        {
            writer.WriteLine($"{DateTime.Now}: {message}");
        }
    }
}