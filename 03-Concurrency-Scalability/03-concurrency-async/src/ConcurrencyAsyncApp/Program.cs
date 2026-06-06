using System.Collections.Concurrent;

var results = new ConcurrentBag<int>();
var numbers = Enumerable.Range(1, 100).ToList();

var tasks = numbers.Select(async number =>
{
    await Task.Delay(10);
    results.Add(number * 2);
}).ToList();

await Task.WhenAll(tasks);

Console.WriteLine($"Count: {results.Count}");
Console.WriteLine($"Sum: {results.Sum()}");
