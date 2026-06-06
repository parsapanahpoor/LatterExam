# چالش مدیریت همزمانی و مقیاس‌پذیری درخواست‌ها در Minimal API

## اطلاعات آزمون
- **دسته‌بندی:** Concurrency-Scalability
- **نوع پروژه:** Minimal API (.NET 9)
- **مدت آزمون:** ۶۰ دقیقه

## آدرس ریپازیتوری
- **URL:** https://git.arzyabit.ir/dotin/Dotnet9ConcurrencyScalabilityChallenge1137
- **Branch:** `main`

## شرح مسئله
شما باید یک Minimal API ساده با .NET 9 پیاده‌سازی کنید که قابلیت پاسخ‌دهی به حجم بالایی از درخواست‌های همزمان را داشته باشد. در این پروژه باید بدون استفاده از حافظه موقت یا کش، همزمانی (Concurrency) و مقیاس‌پذیری (Scalability) رعایت شود و از بروز مشکلاتی مانند **Race Condition** یا کندی شدید جلوگیری گردد.

API دو endpoint دارد:
- `POST /add` — افزودن یک عدد (از form با کلید `number`)
- `GET /sum` — بازگرداندن مجموع اعداد ذخیره‌شده

در کد اولیه از `List<int>` مشترک استفاده شده که thread-safe نیست و در بار بالا دچار Race Condition می‌شود.

## چالش‌های اصلی
- جلوگیری از Race Condition در دسترسی همزمان به لیست مشترک
- حفظ عملکرد صحیح در بار بالا و همزمانی بالا

## الزامات و محدودیت‌ها
- فریم‌ورک: **.NET 9** با ساختار SDK-style
- **بدون cache** و بدون حافظه موقت
- فقط پکیج‌های پایه .NET
- قابل build و run با `dotnet build` و `dotnet run`

## راهنمای شروع
```bash
dotnet run
curl -X POST -F "number=5" http://localhost:5000/add
curl http://localhost:5000/sum
```
