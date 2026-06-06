using SecureTransactionApp;

var dbPath = Path.Combine(AppContext.BaseDirectory, "secure-transactions.db");
var repository = new SecureTransactionRepository($"Data Source={dbPath}");
repository.Initialize();

using var crypto = new TransactionCryptoService(SecureKeyProvider.FromEnvironment());

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1) Register encrypted transaction");
    Console.WriteLine("2) Decrypt transaction");
    Console.WriteLine("3) Exit");
    Console.Write("Choice: ");
    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            Console.Write("Account number: ");
            var account = Console.ReadLine() ?? string.Empty;
            Console.Write("Amount: ");
            if (!decimal.TryParse(Console.ReadLine(), out var amount))
            {
                Console.WriteLine("Invalid amount.");
                break;
            }

            Console.Write("Description: ");
            var description = Console.ReadLine() ?? string.Empty;

            var encrypted = crypto.EncryptTransaction(account, amount, description);
            var id = repository.Store(encrypted);
            Console.WriteLine($"Transaction stored with id {id}. Only ciphertext exists in database.");
            break;

        case "2":
            Console.Write("Transaction id: ");
            if (!long.TryParse(Console.ReadLine(), out var transactionId))
            {
                Console.WriteLine("Invalid id.");
                break;
            }

            var payload = repository.ReadEncrypted(transactionId);
            var decrypted = crypto.DecryptTransaction(payload);
            Console.WriteLine($"Decrypted payload: {decrypted}");
            break;

        case "3":
            return;

        default:
            Console.WriteLine("Unknown option.");
            break;
    }
}
