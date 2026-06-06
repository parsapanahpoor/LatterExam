using Microsoft.Data.Sqlite;

namespace DisasterRecoveryApp;

public sealed class DatabaseService(string dbPath)
{
    public string DatabasePath => dbPath;

    public void Initialize()
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS Records (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Content TEXT NOT NULL,
                CreatedAtUtc TEXT NOT NULL
            );
            """;
        command.ExecuteNonQuery();
    }

    public void AddRecord(string title, string content)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Records (Title, Content, CreatedAtUtc)
            VALUES ($title, $content, $createdAt);
            """;
        command.Parameters.AddWithValue("$title", title);
        command.Parameters.AddWithValue("$content", content);
        command.Parameters.AddWithValue("$createdAt", DateTime.UtcNow.ToString("O"));
        command.ExecuteNonQuery();
    }

    public void ListRecords()
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Title, Content, CreatedAtUtc FROM Records ORDER BY Id;";

        using var reader = command.ExecuteReader();
        var found = false;
        while (reader.Read())
        {
            found = true;
            Console.WriteLine($"#{reader.GetInt64(0)} | {reader.GetString(1)} | {reader.GetString(2)} | {reader.GetString(3)}");
        }

        if (!found)
            Console.WriteLine("No records found.");
    }

    private SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();
        return connection;
    }
}

public sealed class BackupService(string dbPath, string backupDirectory)
{
    private readonly TimeSpan _backupInterval = TimeSpan.FromMinutes(1);
    private CancellationTokenSource? _cts;

    public void StartAutomaticBackups()
    {
        Directory.CreateDirectory(backupDirectory);
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        _ = Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    CreateBackup();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Backup] Failed: {ex.Message}");
                }

                await Task.Delay(_backupInterval, token);
            }
        }, token);

        Console.WriteLine($"Automatic backup started every {_backupInterval.TotalMinutes} minute(s).");
    }

    public void StopAutomaticBackups()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    public string CreateBackup()
    {
        if (!File.Exists(dbPath))
            throw new FileNotFoundException("Database file does not exist.", dbPath);

        Directory.CreateDirectory(backupDirectory);
        var backupPath = Path.Combine(
            backupDirectory,
            $"backup-{DateTime.UtcNow:yyyyMMdd-HHmmss}.db");

        File.Copy(dbPath, backupPath, overwrite: false);
        Console.WriteLine($"[Backup] Created: {backupPath}");
        return backupPath;
    }

    public IReadOnlyList<string> ListBackups()
    {
        if (!Directory.Exists(backupDirectory))
            return Array.Empty<string>();

        return Directory.GetFiles(backupDirectory, "backup-*.db")
            .OrderByDescending(File.GetLastWriteTimeUtc)
            .ToArray();
    }

    public void RestoreLatestBackup()
    {
        var latest = ListBackups().FirstOrDefault()
            ?? throw new InvalidOperationException("No backup files available.");

        RestoreFromBackup(latest);
    }

    public void RestoreFromBackup(string backupPath)
    {
        if (!File.Exists(backupPath))
            throw new FileNotFoundException("Backup file not found.", backupPath);

        if (File.Exists(dbPath))
            File.Delete(dbPath);

        File.Copy(backupPath, dbPath, overwrite: true);
        Console.WriteLine($"Database restored from: {backupPath}");
    }
}
