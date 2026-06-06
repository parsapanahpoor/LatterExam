# چالش امنیت رمز عبور (.NET 9)

## اطلاعات آزمون
- **دسته‌بندی:** Security-Encryption
- **نوع پروژه:** Console App (.NET 9)

## آدرس ریپازیتوری
- **URL:** https://git.arzyabit.ir/Dotin/Dotnet9PasswordSecurityChallenge1076
- **Branch:** `main`

## شرح مسئله
یک Console App ساده با .NET 9 پیاده‌سازی شده که اطلاعات کاربر (نام کاربری و رمز عبور) را دریافت و ذخیره می‌کند. پیاده‌سازی فعلی آسیب‌پذیری امنیتی جدی دارد: رمز عبور به‌صورت **plain-text** در فایل `credentials.txt` ذخیره می‌شود.

## هدف
- شناسایی آسیب‌پذیری امنیتی
- پیاده‌سازی روش امن برای ذخیره رمز عبور

## کد آسیب‌پذیر (اولیه)
```csharp
File.AppendAllText(path, $"{username}:{password}{Environment.NewLine}");
```

## الزامات و محدودیت‌ها
- فقط قابلیت‌های سازگار با **.NET 9**
- رمز عبور **نباید** plain-text ذخیره شود
- استفاده از **PasswordHasher** یا **PBKDF2**
- ذخیره hash + salt در `credentials.txt`
- بدون cache یا حافظه موقت
- پروژه SDK-style و قابل build/run با `dotnet build`

## راهنمای شروع
1. `dotnet build` و `dotnet run`
2. بررسی کد و شناسایی آسیب‌پذیری
3. اصلاح کد برای ذخیره امن رمز عبور
