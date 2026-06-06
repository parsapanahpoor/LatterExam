using System.Collections.Concurrent;

var results = new ConcurrentBag<int>();
var numbers = Enumerable.Range(1, 1000).ToList();

var tasks = numbers.Select(n => Task.Run(() =>
{
    Thread.Sleep(1);
    results.Add(n * n);
})).ToArray();

await Task.WhenAll(tasks);

Console.WriteLine($"Count: {results.Count}");
Console.WriteLine($"Sum: {results.Sum()}");
