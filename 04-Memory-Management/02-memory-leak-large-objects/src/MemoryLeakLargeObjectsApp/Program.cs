namespace MemoryLeakLargeObjectsApp;

public class LargeObject : IDisposable
{
    private bool _disposed;

    public byte[]? Data { get; private set; }

    public LargeObject(int sizeInBytes)
    {
        Data = new byte[sizeInBytes];
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Data = null;
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}

public class ObjectManager : IDisposable
{
    private readonly Dictionary<int, LargeObject> _objects = new();
    private bool _disposed;

    public void Add(int id, LargeObject obj)
    {
        _objects[id] = obj;
    }

    public void Remove(int id)
    {
        if (_objects.TryGetValue(id, out var obj))
        {
            obj.Dispose();
            _objects.Remove(id);
        }
    }

    public int Count => _objects.Count;

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        foreach (var obj in _objects.Values)
        {
            obj.Dispose();
        }

        _objects.Clear();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}

public static class Program
{
    private const int ObjectSize = 1024 * 1024;

    public static void Main()
    {
        using var manager = new ObjectManager();

        for (var i = 0; i < 100; i++)
        {
            manager.Add(i, new LargeObject(ObjectSize));
        }

        Console.WriteLine($"Added 100 objects (~{ObjectSize / 1024}KB each). Count: {manager.Count}");

        for (var i = 0; i < 100; i++)
        {
            manager.Remove(i);
        }

        Console.WriteLine($"Removed all objects. Count: {manager.Count}");

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        Console.WriteLine("Memory released. Press any key to exit.");
        Console.ReadKey();
    }
}
