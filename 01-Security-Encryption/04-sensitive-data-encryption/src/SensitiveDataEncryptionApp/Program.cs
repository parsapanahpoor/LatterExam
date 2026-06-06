using SensitiveDataEncryptionApp;

using var encryption = SecureAesService.FromEnvironment();

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1) Encrypt and store sensitive data");
    Console.WriteLine("2) Load and decrypt sensitive data");
    Console.WriteLine("3) Exit");
    Console.Write("Choice: ");
    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            Console.Write("Enter sensitive value: ");
            var value = Console.ReadLine() ?? string.Empty;
            var encrypted = encryption.Encrypt(value);
            SensitiveDataStore.Save(encrypted);
            Console.WriteLine("Data encrypted with AES-GCM (random nonce per operation) and saved.");
            break;

        case "2":
            var payload = SensitiveDataStore.Load();
            var decrypted = encryption.Decrypt(payload);
            Console.WriteLine($"Decrypted value: {decrypted}");
            break;

        case "3":
            return;

        default:
            Console.WriteLine("Unknown option.");
            break;
    }
}
