<div dir="rtl">

# Disaster Recovery و Recoverability در Console App .NET 9

## اطلاعات آزمون
- **دسته‌بندی:** Disaster-Recovery
- **نوع پروژه:** Console App (.NET 9)
- **مدت آزمون:** ۴۰ دقیقه

## آدرس ریپازیتوری
- **URL:** https://testgit.arzyabit.ir/dotin/Dotnet9-Console-Disaster-Recovery-Challenge1738
- **Branch:** `main`
- **رمز:** `JcXx9#@Fczq9DduV`

## شرح مسئله
یک Console App با .NET 9 اطلاعات مهم را در یک فایل دیتابیس ذخیره می‌کند. در صورت وقوع فاجعه (حذف یا خرابی فایل DB)، هیچ راهکار مناسبی برای بازیابی سریع داده‌ها یا تداوم سرویس وجود ندارد.

لازم است راهکاری برای افزایش **Recoverability** و کاهش **Downtime** با در نظر گرفتن الزامات تداوم کسب‌وکار طراحی و پیاده‌سازی شود.

## چالش‌های اصلی
- Disaster Recovery و Recoverability
- پشتیبان‌گیری خودکار از دیتابیس
- بازیابی سریع
- زمان‌بندی backup
- ذخیره نسخه‌های پشتیبان در فایل‌های مجزا
- گزینه بازیابی در منوی برنامه

## الزامات و محدودیت‌ها
- SQLite / فایل DB
- پشتیبان‌گیری scheduled خودکار
- منوی recovery
- **بدون cache** یا حافظه موقت
- کمترین Downtime ممکن

</div>
