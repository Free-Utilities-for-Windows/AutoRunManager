using Microsoft.Win32;

namespace AutoRunManager;

public class StartupManager
{
    private const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string BackupKey = @"Software\AutoRunManager\StartupBackup";

    public static void DisplayStartupPrograms()
    {
        try
        {
            foreach (string programName in RegistryHelper.ReadValueNames(RunKey))
            {
                Console.WriteLine($"{programName} : {RegistryHelper.ReadValue(RunKey, programName)}");
                Console.WriteLine();
            }
        }
        catch (Exception ex)
        {
            Logger.Log($"An error occurred: {ex.Message}");
        }
    }

    public static void AddStartupProgram(string programName, string programPath)
    {
        try
        {
            if (Validator.IsValidFilePath(programPath))
            {
                RegistryHelper.WriteValue(RunKey, programName, programPath);
                Console.WriteLine($"Program {programName} added to startup.");
            }
            else
            {
                Console.WriteLine("Invalid file path.");
            }
        }
        catch (Exception ex)
        {
            Logger.Log($"An error occurred: {ex.Message}");
        }
    }

    public static void RemoveStartupProgram(string programName)
    {
        try
        {
            RegistryHelper.DeleteValue(RunKey, programName);
            Console.WriteLine($"Program {programName} removed from startup.");
        }
        catch (Exception ex)
        {
            Logger.Log($"An error occurred: {ex.Message}");
        }
    }

    public static void BackupStartupPrograms()
    {
        try
        {
            if (!RegistryHelper.KeyExists(BackupKey))
            {
                Registry.CurrentUser.CreateSubKey(BackupKey);
            }

            foreach (string programName in RegistryHelper.ReadValueNames(RunKey))
            {
                object programPath = RegistryHelper.ReadValue(RunKey, programName);
                RegistryHelper.WriteValue(BackupKey, programName, programPath);
            }

            Console.WriteLine("Startup programs backed up.");
        }
        catch (Exception ex)
        {
            Logger.Log($"An error occurred: {ex.Message}");
        }
    }

    public static void RestoreStartupPrograms()
    {
        try
        {
            if (!RegistryHelper.KeyExists(BackupKey))
            {
                Console.WriteLine("No backup found.");
                return;
            }

            foreach (string programName in RegistryHelper.ReadValueNames(BackupKey))
            {
                object programPath = RegistryHelper.ReadValue(BackupKey, programName);
                RegistryHelper.WriteValue(RunKey, programName, programPath);
            }

            Console.WriteLine("Startup programs restored.");
        }
        catch (Exception ex)
        {
            Logger.Log($"An error occurred: {ex.Message}");
        }
    }

    public static void CleanupStartupPrograms()
    {
        try
        {
            RegistryHelper.DeleteKey(RunKey);
            Console.WriteLine("Startup programs cleaned up.");
        }
        catch (Exception ex)
        {
            Logger.Log($"An error occurred: {ex.Message}");
        }
    }
}