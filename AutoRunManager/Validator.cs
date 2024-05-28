namespace AutoRunManager;

public class Validator
{
    public static bool IsValidFilePath(string path)
    {
        try
        {
            var firstQuoteIndex = path.IndexOf('"');
            var lastQuoteIndex = path.LastIndexOf('"');

            if (firstQuoteIndex == -1 || lastQuoteIndex == -1)
            {
                return File.Exists(path);
            }

            var filePath = path.Substring(firstQuoteIndex + 1, lastQuoteIndex - firstQuoteIndex - 1);

            return File.Exists(filePath);
        }
        catch
        {
            return false;
        }
    }
}