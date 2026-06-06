<div dir="rtl">

# راه‌حل: Disaster Recovery Console App

## گام ۱ — ساختار دیتابیس
SQLite فایل `app-data.db` با جدول `Records` (Id, Title, Content, CreatedAtUtc).

## گام ۲ — پشتیبان‌گیری خودکار
`BackupService` هر ۱ دقیقه (قابل تنظیم) فایل DB را به پوشه `backups/` کپی می‌کند:
```
backup-20250606-143000.db
```
- بدون cache: مستقیم `File.Copy` از فایل DB
- backup روی فایل جداگانه

## گام ۳ — پشتیبان دستی
گزینه منو «Create manual backup» برای backup فوری.

## گام ۴ — بازیابی
- **Restore from latest backup:** آخرین فایل backup
- **Restore from specific backup:** مسیر دلخواه

فرآیند restore:
1. حذف DB فعلی (در صورت وجود)
2. کپی backup به مسیر DB اصلی

## گام ۵ — منوی کاربر
```
1) Add record
2) List records
3) Create manual backup
4) List backups
5) Restore from latest backup
6) Restore from specific backup
7) Exit
```

## گام ۶ — اجرا
```bash
cd src/DisasterRecoveryApp
dotnet build
dotnet run
```

## سناریوی تست فاجعه
1. چند record اضافه کنید.
2. منتظر backup خودکار بمانید یا manual backup بزنید.
3. فایل `app-data.db` را حذف کنید.
4. گزینه ۵ (restore) را اجرا کنید.
5. records باید برگردند.

## چرا بدون cache؟
داده همیشه از فایل DB خوانده می‌شود. backup هم مستقیم از فایل فیزیکی گرفته می‌شود — ساده و قابل اعتماد برای DR.

</div>
