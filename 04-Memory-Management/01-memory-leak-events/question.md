<div dir="rtl">

# چالش مدیریت حافظه و Memory Leak در رویدادها

## اطلاعات آزمون
- **دسته‌بندی:** Memory-Management
- **نوع پروژه:** Console App (.NET 9)
- **مدت آزمون:** ۳۰ دقیقه

## آدرس ریپازیتوری
- **URL:** https://git.arzyabit.ir/dotin/Dotnet9MemoryLeakEventSample1139
- **Branch:** `main`

## شرح مسئله
در این پروژه یک اپلیکیشن کنسولی با .NET 9 پیاده‌سازی شده که شامل یک **EventPublisher** (ناشر رویداد) و **EventSubscriber** (مشترک رویداد) می‌باشد. وظیفه شما شناسایی و رفع مشکل **Memory Leak** ناشی از مدیریت نادرست رویدادها است.

در کد اولیه، `EventSubscriber` در سازنده به رویداد `ProcessCompleted` subscribe می‌کند اما هرگز unsubscribe نمی‌کند. در حلقه ۱۰٬۰۰۰ باره، هر بار subscriber جدید ساخته می‌شود و به publisher متصل می‌ماند.

## چالش‌های اصلی
- Memory Leak ناشی از ثبت رویداد بدون لغو ثبت (unsubscribe)
- آزادسازی اشیاء توسط Garbage Collector پس از رفع مشکل

## الزامات و محدودیت‌ها
- فریم‌ورک: **.NET 9**
- بدون cache و بدون حافظه موقت
- فقط پکیج‌های پایه .NET
- اجرا با `dotnet build` و `dotnet run`

## راهنمای شروع
```bash
dotnet build
dotnet run
```

پس از رفع leak، مصرف حافظه باید قابل کنترل باشد و اشیاء غیرضروری آزاد شوند.

</div>
