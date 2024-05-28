namespace AutoRunManager;

public static class Printer
{
    public static void StartPage()
    {
        Console.ForegroundColor = ConsoleColor.DarkMagenta;

        string title = @"
    _         _        ____              __  __                                   
   / \  _   _| |_ ___ |  _ \ _   _ _ __ |  \/  | __ _ _ __   __ _  __ _  ___ _ __ 
  / _ \| | | | __/ _ \| |_) | | | | '_ \| |\/| |/ _` | '_ \ / _` |/ _` |/ _ \ '__|
 / ___ \ |_| | || (_) |  _ <| |_| | | | | |  | | (_| | | | | (_| | (_| |  __/ |   
/_/   \_\__,_|\__\___/|_| \_\\__,_|_| |_|_|  |_|\__,_|_| |_|\__,_|\__, |\___|_|   
                                                                  |___/                                                              
";
        Console.WriteLine(title);
        
        Console.ResetColor();
    }
}