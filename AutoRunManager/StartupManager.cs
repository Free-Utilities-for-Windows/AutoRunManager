using Microsoft.Win32;

namespace AutoRunManager;

public class StartupManager
{
    private const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string DisabledKey = @"Software\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run";
    private const string BackupKey = @"Software\AutoRunManager\StartupBackup";

    public static List<StartupProgram> GetStartupPrograms()
    {
        var programs = new List<StartupProgram>();

        if (!RegistryHelper.IsWindows()) return programs;

        try
        {
            using (var runKey = Registry.CurrentUser.OpenSubKey(RunKey))
            using (var disabledKey = Registry.CurrentUser.OpenSubKey(DisabledKey))
            {
                if (runKey != null)
                {
                    foreach (string name in runKey.GetValueNames())
                    {
                        var path = runKey.GetValue(name)?.ToString();
                        var isEnabled = true;

                        if (disabledKey != null)
                        {
                            var state = disabledKey.GetValue(name) as byte[];
                            if (state != null && state.Length > 0)
                            {
                                isEnabled = state[0] != 3;
                            }
                        }

                        programs.Add(new StartupProgram
                        {
                            Name = name,
                            Path = path,
                            IsEnabled = isEnabled
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            StaticFileLogger.LogError($"Error getting startup programs: {ex.Message}");
        }

        return programs;
    }

    public static void DisplayStartupPrograms()
    {
        var programs = GetStartupPrograms();

        Console.WriteLine("\nStartup Programs:");
        Console.WriteLine("----------------------------------------");
        Console.WriteLine("Name                  Status    Path");
        Console.WriteLine("----------------------------------------");

        foreach (var program in programs)
        {
            Console.WriteLine($"{program.Name,-20} {(program.IsEnabled ? "Enabled " : "Disabled")} {program.Path}");
        }
    }

    public static void SetStartupProgramState(string programName, bool enable)
    {
        if (!RegistryHelper.IsWindows()) return;

        try
        {
            using (var key = Registry.CurrentUser.CreateSubKey(DisabledKey, true))
            {
                if (key != null)
                {
                    byte[] state = new byte[12];
                    state[0] = (byte)(enable ? 2 : 3);
                    key.SetValue(programName, state, RegistryValueKind.Binary);

                    Console.WriteLine($"Program {programName} has been {(enable ? "enabled" : "disabled")}");
                    StaticFileLogger.LogInformation(
                        $"Program {programName} state changed to {(enable ? "enabled" : "disabled")}");
                }
            }
        }
        catch (Exception ex)
        {
            StaticFileLogger.LogError($"Error setting program state: {ex.Message}");
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
                SetStartupProgramState(programName, true);
                Console.WriteLine($"Program {programName} added to startup and enabled.");
                StaticFileLogger.LogInformation($"Program {programName} added to startup with path {programPath}");
            }
            else
            {
                Console.WriteLine("Invalid file path.");
                StaticFileLogger.LogError("Invalid file path entered.");
            }
        }
        catch (Exception ex)
        {
            StaticFileLogger.LogError($"Error adding startup program: {ex.Message}");
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
            using (var backupKey = Registry.CurrentUser.CreateSubKey(BackupKey))
            {
                if (backupKey == null)
                {
                    throw new Exception("Failed to create backup key");
                }

                var programs = GetStartupPrograms();

                foreach (var valueName in backupKey.GetValueNames())
                {
                    backupKey.DeleteValue(valueName);
                }

                foreach (var program in programs)
                {
                    backupKey.SetValue(program.Name, program.Path);

                    using (var stateKey = Registry.CurrentUser.CreateSubKey(BackupKey + "\\State"))
                    {
                        byte[] state = new byte[12];
                        state[0] = (byte)(program.IsEnabled ? 2 : 3);
                        stateKey.SetValue(program.Name, state, RegistryValueKind.Binary);
                    }
                }

                Console.WriteLine("Startup programs backed up successfully.");
                StaticFileLogger.LogInformation("Startup programs backed up successfully.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to backup startup programs: {ex.Message}");
            StaticFileLogger.LogError($"Backup failed: {ex.Message}");
        }
    }

    public static void RestoreStartupPrograms()
    {
        if (!RegistryHelper.IsWindows()) return;

        try
        {
            using (var backupKey = Registry.CurrentUser.OpenSubKey(BackupKey))
            {
                if (backupKey == null)
                {
                    throw new Exception("No backup found");
                }
                
                using (var runKey = Registry.CurrentUser.OpenSubKey(RunKey, true))
                {
                    if (runKey != null)
                    {
                        foreach (var valueName in runKey.GetValueNames())
                        {
                            runKey.DeleteValue(valueName);
                        }
                    }
                }

                foreach (var programName in backupKey.GetValueNames())
                {
                    var programPath = backupKey.GetValue(programName)?.ToString();
                    if (!string.IsNullOrEmpty(programPath))
                    {
                        AddStartupProgram(programName, programPath);

                        using (var stateKey = Registry.CurrentUser.OpenSubKey(BackupKey + "\\State"))
                        {
                            if (stateKey != null)
                            {
                                var state = stateKey.GetValue(programName) as byte[];
                                if (state != null && state.Length > 0)
                                {
                                    SetStartupProgramState(programName, state[0] == 2);
                                }
                            }
                        }
                    }
                }

                Console.WriteLine("Startup programs restored successfully.");
                StaticFileLogger.LogInformation("Startup programs restored successfully.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to restore startup programs: {ex.Message}");
            StaticFileLogger.LogError($"Restore failed: {ex.Message}");
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
                using (RegistryKey remoteRunKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.CurrentUser, machineName)
                           .OpenSubKey(RunKey))
                {
                    if (remoteRunKey != null)
                    {
                        foreach (string programName in remoteRunKey.GetValueNames())
                        {
                            Console.WriteLine($"{programName} : {remoteRunKey.GetValue(programName)}");
                            StaticFileLogger.LogInformation(
                                $"Displayed remote startup program: {programName} : {remoteRunKey.GetValue(programName)}");
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