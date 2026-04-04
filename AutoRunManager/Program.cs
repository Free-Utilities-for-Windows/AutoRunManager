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
                    Console.WriteLine("4. Enable/Disable a startup program");
                    Console.WriteLine("5. Backup startup programs");
                    Console.WriteLine("6. Restore startup programs");
                    Console.WriteLine("7. Cleanup startup programs");
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
                            StartupManager.DisplayStartupPrograms();
                            Console.Write("Enter program name to remove: ");
                            string removeName = Console.ReadLine();
                            StaticFileLogger.LogInformation($"Removing program from startup: {removeName}");
                            StartupManager.RemoveStartupProgram(removeName);
                            break;
                        case 4:
                            StartupManager.DisplayStartupPrograms();
                            Console.Write("Enter program name: ");
                            string toggleName = Console.ReadLine();
                            Console.Write("Enable or Disable? (e/d): ");
                            string toggleChoice = Console.ReadLine()?.Trim().ToLower();
                            if (toggleChoice == "e" || toggleChoice == "d")
                            {
                                StaticFileLogger.LogInformation($"Toggling program state: {toggleName}");
                                StartupManager.SetStartupProgramState(toggleName, toggleChoice == "e");
                            }
                            else
                            {
                                Console.WriteLine("Invalid input. Enter 'e' to enable or 'd' to disable.");
                            }

                            break;
                        case 5:
                            StaticFileLogger.LogInformation("Backing up startup programs.");
                            StartupManager.BackupStartupPrograms();
                            break;
                        case 6:
                            StaticFileLogger.LogInformation("Restoring startup programs.");
                            StartupManager.RestoreStartupPrograms();
                            break;
                        case 7:
                            StaticFileLogger.LogInformation("Cleaning up startup programs.");
                            StartupManager.CleanupStartupPrograms();
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