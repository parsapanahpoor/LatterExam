var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles");

if (!Directory.Exists(directoryPath))
{
    Directory.CreateDirectory(directoryPath);
    for (var i = 0; i < 20; i++)
    {
        var path = Path.Combine(directoryPath, $"File{i}.txt");
        await File.WriteAllTextAsync(path, $"Initial content {i}");
    }

    Console.WriteLine("Created TestFiles directory with 20 text files.");
}

var filePaths = Directory.GetFiles(directoryPath, "*.txt");
var tasks = filePaths.Select(ProcessFileAsync).ToArray();

await Task.WhenAll(tasks);

Console.WriteLine($"Processed {filePaths.Length} files in parallel.");
Console.WriteLine($"Sample: {await File.ReadAllTextAsync(filePaths[0])}");

static async Task ProcessFileAsync(string filePath)
{
    var content = await File.ReadAllTextAsync(filePath);
    var processed = content.ToUpperInvariant();
    await File.WriteAllTextAsync(filePath, processed);
}
