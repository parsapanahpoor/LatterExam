using Microsoft.Data.Sqlite;

namespace SecureTransactionApp;

public sealed class SecureTransactionRepository(string connectionString)
{
    public void Initialize()
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS SecureTransactions (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                EncryptedPayload BLOB NOT NULL,
                StoredAtUtc TEXT NOT NULL
            );
            """;
        command.ExecuteNonQuery();
    }

    public long Store(byte[] encryptedPayload)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO SecureTransactions (EncryptedPayload, StoredAtUtc)
            VALUES ($payload, $storedAt);
            SELECT last_insert_rowid();
            """;
        command.Parameters.AddWithValue("$payload", encryptedPayload);
        command.Parameters.AddWithValue("$storedAt", DateTime.UtcNow.ToString("O"));

        return (long)(command.ExecuteScalar() ?? 0L);
    }

    public byte[] ReadEncrypted(long id)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT EncryptedPayload FROM SecureTransactions WHERE Id = $id;";
        command.Parameters.AddWithValue("$id", id);

        var value = command.ExecuteScalar();
        if (value is null or DBNull)
            throw new KeyNotFoundException($"Transaction {id} not found.");

        return (byte[])value;
    }
}
