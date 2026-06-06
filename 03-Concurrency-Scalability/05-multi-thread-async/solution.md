# راه‌حل: چالش Multi-threading

## گام ۱ — شناسایی مشکل
```csharp
static List<int> results = new();
// در Task.Run:
results.Add(number * number); // Race Condition
```

`Task.Run` کار را روی thread pool اجرا می‌کند. چند thread همزمان به `List<int>` می‌نویسند.

## گام ۲ — استفاده از ConcurrentBag
```csharp
using System.Collections.Concurrent;

static ConcurrentBag<int> results = new();
```

یا با `lock`:
```csharp
static readonly object Sync = new();
lock (Sync) { results.Add(number * number); }
```

## گام ۳ — ساخت Taskها با Task.Run
```csharp
var numbers = Enumerable.Range(1, 1000).ToList();
var tasks = numbers.Select(n => Task.Run(() => ProcessNumber(n))).ToArray();
await Task.WhenAll(tasks);
```

`Task.Run` برای کار CPU-bound مناسب است.

## گام ۴ — بررسی نتیجه
- `results.Count` باید ۱۰۰۰ باشد
- مجموع باید Σ(n²) برای n=1..1000 باشد (= 332833500)

## گام ۵ — اجرا
```bash
cd src/MultiThreadAsyncApp
dotnet build
dotnet run
```

## نکات
- `Task.Run` + `ConcurrentBag` الگوی رایج برای پردازش موازی CPU-bound است.
- از `async void` در این سناریو پرهیز کنید.
