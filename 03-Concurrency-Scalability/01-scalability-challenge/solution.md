<div dir="rtl">

# راه‌حل: چالش مقیاس‌پذیری Minimal API

## گام ۱ — شناسایی مشکل
کد اولیه در هر تکرار حلقه منتظر اتمام `SimulateWork` می‌ماند:

```csharp
for (int i = 0; i < count; i++)
{
    results.Add(await SimulateWork(i)); // اجرای متوالی
}
```

با ۱۰٬۰۰۰ تکرار و `Task.Delay(2)`، زمان کل حدود ۲۰ ثانیه است.

## گام ۲ — ایجاد Taskها به‌صورت موازی
به‌جای `await` داخل حلقه، ابتدا همه Taskها ساخته می‌شوند:

```csharp
var tasks = Enumerable.Range(0, count).Select(SimulateWork);
var results = await Task.WhenAll(tasks);
```

## گام ۳ — انتظار همزمان با Task.WhenAll
`Task.WhenAll` همه وظایف را همزمان اجرا می‌کند و منتظر اتمام همه می‌ماند. زمان کل نزدیک به طولانی‌ترین Task (چند میلی‌ثانیه + overhead) می‌شود، نه مجموع همه.

## گام ۴ — بازگرداندن نتیجه
لیست نتایج مستقیماً به JSON تبدیل و برگردانده می‌شود:

```csharp
await context.Response.WriteAsJsonAsync(results);
```

## گام ۵ — اجرا و تست
```bash
cd src/ScalabilityChallengeApp
dotnet build
dotnet run
```

سپس درخواست POST به `/process` ارسال کنید. زمان پاسخ باید از ~۲۰ ثانیه به چند ثانیه یا کمتر کاهش یابد.

## نکات
- از cache استفاده نشده است.
- `SimulateWork` بدون تغییر باقی می‌ماند؛ فقط نحوه فراخوانی اصلاح شده است.
- این الگو برای I/O-bound و CPU-bound سبک هر دو مناسب است.

</div>
