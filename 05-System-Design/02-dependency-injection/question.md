<div dir="rtl">

# اصلاح Dependency Injection و Lifetime

## موضوع
در یک برنامه Console با DI، lifetime سرویس‌ها اشتباه تنظیم شده است. باید آن را اصلاح کنید.

## ساختار
- `IRepository` / `Repository` — دسترسی به داده
- `IService` / `Service` — منطق کسب‌وکار که به Repository وابسته است

## مشکل
Lifetime فعلی باعث می‌شود چند سرویس instance مشترک Repository دریافت کنند و رفتار نادرست (مثلاً state مشترک) رخ دهد.

## الزامات اصلاح
- `Repository` باید **Transient** یا **Scoped** باشد.
- `Service` باید **Transient** باشد تا هر سرویس instance جدید Repository دریافت کند.
- برنامه باید با **.NET 9** build و اجرا شود.

## خروجی مورد انتظار
- Console app که تفاوت lifetime صحیح را نشان می‌دهد
- DI container با lifetime درست

</div>
