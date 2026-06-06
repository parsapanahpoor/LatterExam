<div dir="rtl">

# چالش Multi-threading و Async در .NET 9

## اطلاعات آزمون
- **دسته‌بندی:** Concurrency-Scalability
- **نوع پروژه:** Console App (.NET 9)
- **مدت آزمون:** ۳۰ دقیقه

## شرح مسئله
یک Console App باید **۱۰۰۰ عدد** را با استفاده از `Task.Run` به‌صورت موازی پردازش کند. هر عدد به مربع خود تبدیل و در لیست نتایج ذخیره می‌شود.

در کد اولیه از `List<int>` مشترک استفاده شده و چند thread همزمان به آن می‌نویسند. این باعث **Race Condition** و نتایج نادرست می‌شود.

## چالش‌های اصلی
- پردازش ۱۰۰۰ عدد با `Task.Run` روی thread pool
- رفع Race Condition با `ConcurrentBag` یا `lock`
- انتظار صحیح برای اتمام همه Taskها با `Task.WhenAll`

## الزامات و محدودیت‌ها
- فریم‌ورک: **.NET 9**
- بدون وابستگی خارجی
- بدون cache

## راهنمای شروع
```bash
dotnet build
dotnet run
```

خروجی مورد انتظار: `Count: 1000` و مجموع صحیح مربع اعداد ۱ تا ۱۰۰۰.

</div>
