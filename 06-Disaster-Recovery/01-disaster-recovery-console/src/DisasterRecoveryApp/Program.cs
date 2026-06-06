using DisasterRecoveryApp;

var baseDirectory = AppContext.BaseDirectory;
var dbPath = Path.Combine(baseDirectory, "app-data.db");
var backupDirectory = Path.Combine(baseDirectory, "backups");

var database = new DatabaseService(dbPath);
database.Initialize();

var backupService = new BackupService(dbPath, backupDirectory);
backupService.StartAutomaticBackups();

try
{
    while (true)
    {
        Console.WriteLine();
        Console.WriteLine("1) Add record");
        Console.WriteLine("2) List records");
        Console.WriteLine("3) Create manual backup");
        Console.WriteLine("4) List backups");
        Console.WriteLine("5) Restore from latest backup");
        Console.WriteLine("6) Restore from specific backup");
        Console.WriteLine("7) Exit");
        Console.Write("Choice: ");

        switch (Console.ReadLine())
        {
            case "1":
                Console.Write("Title: ");
                var title = Console.ReadLine() ?? string.Empty;
                Console.Write("Content: ");
                var content = Console.ReadLine() ?? string.Empty;
                database.AddRecord(title, content);
                Console.WriteLine("Record saved.");
                break;

            case "2":
                database.ListRecords();
                break;

            case "3":
                backupService.CreateBackup();
                break;

            case "4":
                foreach (var backup in backupService.ListBackups())
                    Console.WriteLine(backup);
                break;

            case "5":
                backupService.RestoreLatestBackup();
                break;

            case "6":
                Console.Write("Backup file path: ");
                var path = Console.ReadLine() ?? string.Empty;
                backupService.RestoreFromBackup(path);
                break;

            case "7":
                return;

            default:
                Console.WriteLine("Unknown option.");
                break;
        }
    }
}
finally
{
    backupService.StopAutomaticBackups();
}
