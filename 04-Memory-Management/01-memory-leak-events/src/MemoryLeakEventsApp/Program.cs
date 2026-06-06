namespace MemoryLeakEventsApp;

public class EventPublisher
{
    public event EventHandler? ProcessCompleted;

    public void DoProcess()
    {
        Thread.Sleep(1);
        OnProcessCompleted(EventArgs.Empty);
    }

    protected virtual void OnProcessCompleted(EventArgs e)
    {
        ProcessCompleted?.Invoke(this, e);
    }
}

public class EventSubscriber : IDisposable
{
    private readonly EventPublisher _publisher;
    private readonly string _name;
    private bool _disposed;

    public EventSubscriber(string name, EventPublisher publisher)
    {
        _name = name;
        _publisher = publisher;
        _publisher.ProcessCompleted += HandleProcessCompleted;
    }

    private void HandleProcessCompleted(object? sender, EventArgs e)
    {
        Console.WriteLine($"{_name} received event.");
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _publisher.ProcessCompleted -= HandleProcessCompleted;
        _disposed = true;
    }
}

public static class Program
{
    public static void Main()
    {
        for (var i = 0; i < 10000; i++)
        {
            var publisher = new EventPublisher();
            using var subscriber = new EventSubscriber($"Subscriber_{i}", publisher);
            publisher.DoProcess();
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();

        Console.WriteLine("Completed. Press any key to exit.");
        Console.ReadKey();
    }
}
