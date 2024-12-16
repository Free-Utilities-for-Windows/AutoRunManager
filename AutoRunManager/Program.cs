using System;
using Microsoft.Extensions.Logging;

namespace AutoRunManager
{
 class Program
    {
        static void Main(string[] args)
        {
            var administratorChecker = new AdministratorChecker();
            bool isAdmin = administratorChecker.IsCurrentUserAdmin();
            
            StaticFileLogger.LogInformation("Application started");
            
            Printer.StartPage();
            Printer.PrivilegeStatusPrinter(isAdmin);
            
            while (true)
            {
                try
                {
                    Console.WriteLine("1. Display startup programs");
                    Console.WriteLine("2. Add a program to startup");
                    Console.WriteLine("3. Remove a program from startup");
                    Console.WriteLine("4. Backup startup programs");
                    Console.WriteLine("5. Restore startup programs");
                    Console.WriteLine("6. Cleanup startup programs");
                    Console.WriteLine("7. Display remote startup programs");
                    Console.WriteLine("8. Exit");
                    Console.Write("Enter your choice: ");

                    if (!int.TryParse(Console.ReadLine(), out int choice))
                    {
                        Console.WriteLine("Invalid choice. Please enter a number.");
                        StaticFileLogger.LogError("Invalid choice entered.");
                        continue;
                    }

                    switch (choice)
                    {
                        case 1:
                            StaticFileLogger.LogInformation("Displaying startup programs.");
                            StartupManager.DisplayStartupPrograms();
                            break;
                        case 2:
                            var (programName, programPath) = GetProgramDetails();
                            StaticFileLogger.LogInformation($"Adding program to startup: {programName}, {programPath}");
                            StartupManager.AddStartupProgram(programName, programPath);
                            break;
                        case 3:
                            Console.Write("Enter program name to toggle: ");
                            string toggleName = Console.ReadLine();
                            Console.Write("Enable program? (true/false): ");
                            if (bool.TryParse(Console.ReadLine(), out bool enable))
                            {
                                StartupManager.SetStartupProgramState(toggleName, enable);
                            }
                            break;
                        case 4:
                            StaticFileLogger.LogInformation("Backing up startup programs.");
                            StartupManager.BackupStartupPrograms();
                            break;
                        case 5:
                            StaticFileLogger.LogInformation("Restoring startup programs.");
                            StartupManager.RestoreStartupPrograms();
                            break;
                        case 6:
                            StaticFileLogger.LogInformation("Cleaning up startup programs.");
                            StartupManager.CleanupStartupPrograms();
                            break;
                        case 7:
                            Console.Write("Enter remote machine name: ");
                            string machineName = Console.ReadLine();
                            StaticFileLogger.LogInformation($"Displaying remote startup programs for machine: {machineName}");
                            StartupManager.DisplayRemoteStartupPrograms(machineName);
                            break;
                        case 8:
                            StaticFileLogger.LogInformation("Exiting application.");
                            return;
                        default:
                            Console.WriteLine("Invalid choice.");
                            StaticFileLogger.LogError("Invalid choice entered.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    StaticFileLogger.LogError($"An error occurred: {ex.Message}");
                }

                Console.WriteLine();
            }
        }

        static string GetProgramName()
        {
            Console.Write("Enter program name: (Example: WingetUI) ");
            return Console.ReadLine();
        }

        static (string, string) GetProgramDetails()
        {
            string programName = GetProgramName();
            Console.Write("Enter program path: (Example: \"C:\\Program Files\\WingetUI\\WingetUI.exe\") ");
            string programPath = Console.ReadLine();
            return (programName, programPath);
        }
    }
}