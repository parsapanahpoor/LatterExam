<div dir="rtl">

# راه‌حل: چالش Async و همزمانی

## گام ۱ — شناسایی مشکل
```csharp
static List<int> results = new List<int>();
// ...
results.Add(number * 2); // Race Condition
```

چند Task همزمان به `List<int>` می‌نویسند. `List<T>` thread-safe نیست.

## گام ۲ — جایگزینی با `ConcurrentBag<int>`
```csharp
using System.Collections.Concurrent;

static ConcurrentBag<int> results = new();
// ...
results.Add(number * 2);
```

`ConcurrentBag<T>` برای افزودن همزمان از چند thread طراحی شده است.

**جایگزین:** استفاده از `lock`:
```csharp
static readonly object Sync = new();
lock (Sync) { results.Add(number * 2); }
```

## گام ۳ — حفظ `Task.WhenAll`
کد اولیه `Task.WhenAll` را درست استفاده می‌کند:
```csharp
var tasks = new List<Task>();
foreach (var number in numbers)
    tasks.Add(ProcessNumberAsync(number));
await Task.WhenAll(tasks);
```

این بخش نیازی به تغییر ندارد.

## گام ۴ — بررسی نتیجه
پس از رفع Race Condition:
- تعداد عناصر در `results` باید ۱۰۰ باشد
- `Sum` باید `10100` باشد (۲+۴+۶+...+۲۰۰)

## گام ۵ — اجرا
```bash
cd src/ConcurrencyAsyncApp
dotnet build
dotnet run
```

## نکات
- `ConcurrentBag` ترتیب را تضمین نمی‌کند اما برای جمع‌آوری نتایج کافی است.
- `await Task.WhenAll` از خروج زودهنگام `Main` قبل از اتمام همه Taskها جلوگیری می‌کند.

</div>
