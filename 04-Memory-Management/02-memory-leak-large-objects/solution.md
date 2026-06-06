# راه‌حل: Memory Leak در اشیای حجیم

## گام ۱ — شناسایی مشکل
```csharp
public void Remove(int id)
{
    _objects.Remove(id); // فقط کلید حذف می‌شود
}
```

اگر `LargeObject` قبل از حذف dispose نشود، آرایه `byte[]` بزرگ در حافظه باقی می‌ماند تا GC آن را جمع‌آوری کند. در برنامه‌های long-running این باعث رشد مداوم حافظه می‌شود.

## گام ۲ — پیاده‌سازی IDisposable در LargeObject
```csharp
public class LargeObject : IDisposable
{
    public byte[]? Data { get; private set; }

    public void Dispose()
    {
        Data = null; // آزادسازی reference
        GC.SuppressFinalize(this);
    }
}
```

## گام ۳ — Dispose در Remove
```csharp
public void Remove(int id)
{
    if (_objects.TryGetValue(id, out var obj))
    {
        obj.Dispose();
        _objects.Remove(id);
    }
}
```

## گام ۴ — Dispose در Clear
```csharp
public void Clear()
{
    foreach (var obj in _objects.Values)
        obj.Dispose();
    _objects.Clear();
}
```

## گام ۵ — اجرا و تأیید
```bash
cd src/MemoryLeakLargeObjectsApp
dotnet build
dotnet run
```

برنامه ۱۰۰ شیء ۱MB می‌سازد، همه را حذف می‌کند و GC را فراخوانی می‌کند. پس از رفع leak، حافظه باید آزاد شود.

## نکات
- اشیای بزرگ‌تر از ۸۵KB در LOH (Large Object Heap) قرار می‌گیرند.
- آزادسازی صریح reference (`= null`) به GC کمک می‌کند.
- برای منابع unmanaged همیشه `IDisposable` پیاده کنید.
