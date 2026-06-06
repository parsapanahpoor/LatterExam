using ExceptionHandlingDemo;
using Microsoft.Extensions.Logging;

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddSimpleConsole(options =>
    {
        options.SingleLine = true;
        options.TimestampFormat = "HH:mm:ss ";
    });
    builder.SetMinimumLevel(LogLevel.Information);
});

var logger = loggerFactory.CreateLogger("ExceptionHandlingDemo");

var inputs = new List<string> { "10", "abc", "25", "", "3.5", "7" };

Console.WriteLine("=== تبدیل رشته به عدد و جمع ===\n");
Console.WriteLine($"ورودی‌ها: [{string.Join(", ", inputs.Select(i => $"\"{i}\""))}]\n");

var sum = NumberParser.SumValidNumbers(inputs, logger);

Console.WriteLine($"\n✅ جمع اعداد معتبر: {sum}");
