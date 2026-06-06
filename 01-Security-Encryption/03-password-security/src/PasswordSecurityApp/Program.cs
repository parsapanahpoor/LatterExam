using PasswordSecurityApp;

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1) Save credentials");
    Console.WriteLine("2) Verify credentials");
    Console.WriteLine("3) Exit");
    Console.Write("Choice: ");
    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            Console.Write("Enter username: ");
            var username = Console.ReadLine() ?? string.Empty;
            Console.Write("Enter password: ");
            var password = Console.ReadLine() ?? string.Empty;
            CredentialStore.SaveCredentials(username, password);
            Console.WriteLine("Credentials saved securely (hash + salt).");
            break;

        case "2":
            Console.Write("Enter username: ");
            var verifyUser = Console.ReadLine() ?? string.Empty;
            Console.Write("Enter password: ");
            var verifyPassword = Console.ReadLine() ?? string.Empty;
            var valid = CredentialStore.VerifyCredentials(verifyUser, verifyPassword);
            Console.WriteLine(valid ? "Authentication successful." : "Authentication failed.");
            break;

        case "3":
            return;

        default:
            Console.WriteLine("Unknown option.");
            break;
    }
}
