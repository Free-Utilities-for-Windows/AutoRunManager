using Microsoft.Win32;

namespace AutoRunManager;

public class StartupManager
{
    private const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string BackupKey = @"Software\AutoRunManager\StartupBackup";

    public static void DisplayStartupPrograms()
    {
        if (!RegistryHelper.IsWindows()) return;

        try
        {
            foreach (string programName in RegistryHelper.ReadValueNames(RunKey))
            {
                var value = RegistryHelper.ReadValue(RunKey, programName);
                var valueKind = RegistryHelper.GetValueKind(RunKey, programName);
                Console.WriteLine($"{programName} : {value} (Type: {valueKind})");
                StaticFileLogger.LogInformation($"Displayed startup program: {programName} : {value} (Type: {valueKind})");
                Console.WriteLine();
            }
        }
        catch (Exception ex)
        {
            StaticFileLogger.LogError($"An error occurred: {ex.Message}");
        }
    }

    public static void AddStartupProgram(string programName, string programPath)
    {
        if (!RegistryHelper.IsWindows()) return;

        try
        {
            if (Validator.IsValidFilePath(programPath))
            {
                RegistryHelper.WriteValue(RunKey, programName, programPath);
                Console.WriteLine($"Program {programName} added to startup.");
                StaticFileLogger.LogInformation($"Program {programName} added to startup with path {programPath}.");
            }
            else
            {
                Console.WriteLine("Invalid file path.");
                StaticFileLogger.LogError("Invalid file path entered.");
            }
        }
        catch (Exception ex)
        {
            StaticFileLogger.LogError($"An error occurred: {ex.Message}");
        }
    }

    public static void RemoveStartupProgram(string programName)
    {
        if (!RegistryHelper.IsWindows()) return;

        try
        {
            RegistryHelper.DeleteValue(RunKey, programName);
            Console.WriteLine($"Program {programName} removed from startup.");
            StaticFileLogger.LogInformation($"Program {programName} removed from startup.");
        }
        catch (Exception ex)
        {
            StaticFileLogger.LogError($"An error occurred: {ex.Message}");
        }
    }

    public static void BackupStartupPrograms()
    {
        if (!RegistryHelper.IsWindows()) return;

        try
        {
            if (RegistryHelper.SetRegistryKeyAccessPermissions(BackupKey))
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

                string backupPath = $"HKEY_CURRENT_USER\\{BackupKey}";
                Console.WriteLine($"Startup programs backed up to {backupPath}.");
                StaticFileLogger.LogInformation($"Startup programs backed up to {backupPath}.");
            }
            else
            {
                Console.WriteLine("Failed to set permissions for backup key.");
                StaticFileLogger.LogError("Failed to set permissions for backup key.");
            }
        }
        catch (Exception ex)
        {
            StaticFileLogger.LogError($"An error occurred: {ex.Message}");
        }
    }

    public static void RestoreStartupPrograms()
    {
        if (!RegistryHelper.IsWindows()) return;

        try
        {
            if (!RegistryHelper.KeyExists(BackupKey))
            {
                Console.WriteLine("No backup found.");
                StaticFileLogger.LogInformation("No backup found.");
                return;
            }

            foreach (string programName in RegistryHelper.ReadValueNames(BackupKey))
            {
                object programPath = RegistryHelper.ReadValue(BackupKey, programName);
                RegistryHelper.WriteValue(RunKey, programName, programPath);
            }

            string backupPath = $"HKEY_CURRENT_USER\\{BackupKey}";
            Console.WriteLine($"Startup programs restored from {backupPath}.");
            StaticFileLogger.LogInformation($"Startup programs restored from {backupPath}.");
        }
        catch (Exception ex)
        {
            StaticFileLogger.LogError($"An error occurred: {ex.Message}");
        }
    }

    public static void CleanupStartupPrograms()
    {
        if (!RegistryHelper.IsWindows()) return;

        try
        {
            RegistryHelper.DeleteKey(RunKey);
            Console.WriteLine("Startup programs cleaned up.");
            StaticFileLogger.LogInformation("Startup programs cleaned up.");
        }
        catch (Exception ex)
        {
            StaticFileLogger.LogError($"An error occurred: {ex.Message}");
        }
    }

    public static void DisplayRemoteStartupPrograms(string machineName)
    {
        if (!RegistryHelper.IsWindows()) return;

        try
        {
            if (RegistryHelper.OpenRemoteBaseKey(machineName))
            {
                using (RegistryKey remoteRunKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.CurrentUser, machineName).OpenSubKey(RunKey))
                {
                    if (remoteRunKey != null)
                    {
                        foreach (string programName in remoteRunKey.GetValueNames())
                        {
                            Console.WriteLine($"{programName} : {remoteRunKey.GetValue(programName)}");
                            StaticFileLogger.LogInformation($"Displayed remote startup program: {programName} : {remoteRunKey.GetValue(programName)}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Failed to open remote run key.");
                        StaticFileLogger.LogError("Failed to open remote run key.");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Could not connect to remote machine: {machineName}");
                StaticFileLogger.LogError($"Could not connect to remote machine: {machineName}");
            }
        }
        catch (Exception ex)
        {
            StaticFileLogger.LogError($"An error occurred: {ex.Message}");
        }
    }
}