using System;

namespace AutoRunManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Printer.StartPage();
            
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
                    Console.WriteLine("7. Exit");
                    Console.Write("Enter your choice: ");

                    if (!int.TryParse(Console.ReadLine(), out int choice))
                    {
                        Console.WriteLine("Invalid choice. Please enter a number.");
                        continue;
                    }

                    switch (choice)
                    {
                        case 1:
                            StartupManager.DisplayStartupPrograms();
                            break;
                        case 2:
                            var (programName, programPath) = GetProgramDetails();
                            StartupManager.AddStartupProgram(programName, programPath);
                            break;
                        case 3:
                            programName = GetProgramName();
                            StartupManager.RemoveStartupProgram(programName);
                            break;
                        case 4:
                            StartupManager.BackupStartupPrograms();
                            break;
                        case 5:
                            StartupManager.RestoreStartupPrograms();
                            break;
                        case 6:
                            StartupManager.CleanupStartupPrograms();
                            break;
                        case 7:
                            return;
                        default:
                            Console.WriteLine("Invalid choice.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"An error occurred: {ex.Message}");
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