using TransactionPrivacyApp;

var dbPath = Path.Combine(AppContext.BaseDirectory, "transactions.db");
var repository = new TransactionRepository($"Data Source={dbPath}");
repository.Initialize();

using var encryption = AesGcmEncryptionService.FromEnvironment();

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1) Add transaction");
    Console.WriteLine("2) View transaction");
    Console.WriteLine("3) Exit");
    Console.Write("Choice: ");
    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            Console.Write("Transaction details: ");
            var input = Console.ReadLine() ?? string.Empty;
            var encrypted = encryption.Encrypt(input);
            var id = repository.SaveEncrypted(encrypted);
            Console.WriteLine($"Saved encrypted transaction #{id}.");
            break;

        case "2":
            Console.Write("Transaction id: ");
            if (!long.TryParse(Console.ReadLine(), out var transactionId))
            {
                Console.WriteLine("Invalid id.");
                break;
            }

            var payload = repository.GetEncryptedPayload(transactionId);
            var decrypted = encryption.Decrypt(payload);
            Console.WriteLine($"Decrypted transaction: {decrypted}");
            break;

        case "3":
            return;

        default:
            Console.WriteLine("Unknown option.");
            break;
    }
}
