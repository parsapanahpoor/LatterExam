using Microsoft.Data.Sqlite;

namespace TransactionPrivacyApp;

public sealed class TransactionRepository(string connectionString)
{
    public void Initialize()
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS Transactions (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                EncryptedPayload BLOB NOT NULL,
                CreatedAtUtc TEXT NOT NULL
            );
            """;
        command.ExecuteNonQuery();
    }

    public long SaveEncrypted(byte[] encryptedPayload)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Transactions (EncryptedPayload, CreatedAtUtc)
            VALUES ($payload, $createdAt);
            SELECT last_insert_rowid();
            """;
        command.Parameters.AddWithValue("$payload", encryptedPayload);
        command.Parameters.AddWithValue("$createdAt", DateTime.UtcNow.ToString("O"));

        return (long)(command.ExecuteScalar() ?? 0L);
    }

    public byte[] GetEncryptedPayload(long id)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT EncryptedPayload FROM Transactions WHERE Id = $id;";
        command.Parameters.AddWithValue("$id", id);

        var result = command.ExecuteScalar();
        if (result is null or DBNull)
            throw new KeyNotFoundException($"Transaction {id} was not found.");

        return (byte[])result;
    }
}
