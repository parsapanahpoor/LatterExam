<div dir="rtl">

# چالش Memory Leak در مدیریت اشیای حجیم

## اطلاعات آزمون
- **دسته‌بندی:** Memory-Management
- **نوع پروژه:** Console App (.NET 9)

## آدرس ریپازیتوری
- **URL:** https://git.arzyabit.ir/dotin/Dotnet9MemoryLeakChallenge1073
- **Branch:** `main`

## شرح مسئله
در این پروژه یک اپلیکیشن Console با .NET 9 پیاده‌سازی شده که به‌صورت عمدی دارای مشکل **Memory Leak** در مدیریت اشیای حجیم است. کلاس `ObjectManager` اشیائی با آرایه‌های بایت بزرگ (`byte[]`) نگه می‌دارد.

در کد اولیه، متد `Remove` فقط کلید را از Dictionary حذف می‌کند اما reference به آرایه بایت بزرگ را آزاد نمی‌کند. هدف شما شناسایی و رفع این مشکل است.

## چالش‌های اصلی
- Memory Leak ناشی از نگهداری reference به اشیای حجیم پس از «حذف»
- پیاده‌سازی disposal صحیح و آزادسازی حافظه

## الزامات و محدودیت‌ها
- فریم‌ورک: **.NET 9**
- بدون cache
- فقط جنبه فنی مدیریت حافظه (بدون سناریوی تجاری)
- اجرا با `dotnet build` و `dotnet run`

## راهنمای شروع
```bash
dotnet build
dotnet run
```

پس از رفع leak، حافظه مصرفی باید پس از حذف اشیاء کاهش یابد.

</div>
