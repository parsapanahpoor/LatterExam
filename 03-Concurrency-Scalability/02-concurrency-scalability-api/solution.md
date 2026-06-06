# راه‌حل: چالش همزمانی Minimal API

## گام ۱ — شناسایی Race Condition
`List<int>` thread-safe نیست. وقتی چند درخواست POST همزمان `numbers.Add()` را فراخوانی کنند، ممکن است:
- داده از دست برود
- `IndexOutOfRangeException` رخ دهد
- مجموع نادرست محاسبه شود

## گام ۲ — انتخاب راهکار همزمانی
دو راهکار معتبر:

### راهکار الف: `lock`
```csharp
private static readonly object Sync = new();
private static readonly List<int> numbers = new();

lock (Sync) { numbers.Add(number); }
lock (Sync) { return numbers.Sum(); }
```

### راهکار ب: `ConcurrentBag<int>`
```csharp
private static readonly ConcurrentBag<int> numbers = new();
numbers.Add(number);
return numbers.Sum();
```

در این راه‌حل از `lock` استفاده شده است چون خواندن مجموع و نوشتن هر دو به‌صورت اتمیک محافظت می‌شوند.

## گام ۳ — محافظت از هر دو endpoint
هم `POST /add` و هم `GET /sum` باید از همان شیء قفل استفاده کنند تا خواندن در حین نوشتن ناسازگار نباشد.

## گام ۴ — اجرا و تست
```bash
cd src/ConcurrencyScalabilityApi
dotnet build
dotnet run
```

چند درخواست همزمان POST ارسال کنید و سپس GET `/sum` را بررسی کنید. مجموع باید دقیق باشد.

## نکات
- از cache استفاده نشده است.
- `lock` ساده‌ترین راه برای حفظ یکپارچگی `List<T>` است.
- برای سناریوهای با بار بسیار بالا می‌توان از `Interlocked.Add` روی یک متغیر `long` نیز استفاده کرد.
