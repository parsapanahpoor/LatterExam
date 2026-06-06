<div dir="rtl">

# راه‌حل: Memory Leak در رویدادها

## گام ۱ — شناسایی علت Leak
```csharp
public EventSubscriber(string name, EventPublisher publisher)
{
    publisher.ProcessCompleted += HandleProcessCompleted; // subscribe
    // هیچ unsubscribe ای وجود ندارد
}
```

وقتی subscriber به رویداد subscribe می‌کند، publisher یک reference به subscriber نگه می‌دارد. حتی اگر متغیر subscriber از scope خارج شود، delegate مانع GC می‌شود.

## گام ۲ — پیاده‌سازی IDisposable
```csharp
public class EventSubscriber : IDisposable
{
    private readonly EventPublisher _publisher;
    private bool _disposed;

    public void Dispose()
    {
        if (_disposed) return;
        _publisher.ProcessCompleted -= HandleProcessCompleted;
        _disposed = true;
    }
}
```

## گام ۳ — استفاده از using در حلقه
```csharp
for (int i = 0; i < 10000; i++)
{
    var publisher = new EventPublisher();
    using (var subscriber = new EventSubscriber($"Subscriber_{i}", publisher))
    {
        publisher.DoProcess();
    }
}
```

پس از خروج از `using`، `Dispose` فراخوانی و subscription لغو می‌شود.

**جایگزین:** unsubscribe مستقیم پس از `DoProcess`:
```csharp
subscriber.Dispose();
```

## گام ۴ — تأیید رفع مشکل
```csharp
GC.Collect();
GC.WaitForPendingFinalizers();
```

پس از رفع leak، subscriberها باید قابل جمع‌آوری باشند.

## گام ۵ — اجرا
```bash
cd src/MemoryLeakEventsApp
dotnet build
dotnet run
```

## نکات
- همیشه برای event subscription های موقت از `IDisposable` یا unsubscribe صریح استفاده کنید.
- این الگو در WPF/WinForms و سرویس‌های long-lived بسیار رایج است.

</div>
