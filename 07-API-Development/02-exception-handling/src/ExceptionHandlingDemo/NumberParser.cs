using Microsoft.Extensions.Logging;

namespace ExceptionHandlingDemo;

public static class NumberParser
{
    public static int SumValidNumbers(IEnumerable<string> inputs, ILogger logger)
    {
        var sum = 0;
        var index = 0;

        foreach (var input in inputs)
        {
            index++;
            if (int.TryParse(input, out var number))
            {
                sum += number;
                logger.LogInformation("ورودی {Index}: '{Input}' -> {Number} (معتبر)", index, input, number);
            }
            else
            {
                logger.LogError("ورودی {Index}: '{Input}' -> نامعتبر، رد شد", index, input);
            }
        }

        return sum;
    }
}
