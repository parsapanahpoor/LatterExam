# راه‌حل: امن‌سازی ذخیره رمز عبور

## گام ۱ — شناسایی مشکل
ذخیره `username:password` در فایل متنی یعنی هر کسی با دسترسی به فایل، رمز واقعی را می‌بیند.

## گام ۲ — انتخاب الگوریتم
از **PBKDF2** (`Rfc2898DeriveBytes.Pbkdf2`) با:
- Salt تصادفی ۱۶ بایتی
- ۱۰۰٬۰۰۰ iteration
- SHA-256
- خروجی hash ۳۲ بایتی

## گام ۳ — فرمت ذخیره‌سازی
هر خط در `credentials.txt`:
```
username:base64(salt):base64(hash)
```

## گام ۴ — ذخیره (Register)
1. Salt تصادفی تولید می‌شود.
2. Hash از password + salt محاسبه می‌شود.
3. فقط username، salt و hash ذخیره می‌شوند.

## گام ۵ — تأیید (Verify)
1. خط مربوط به username خوانده می‌شود.
2. Hash جدید از password واردشده + salt ذخیره‌شده محاسبه می‌شود.
3. مقایسه با `CryptographicOperations.FixedTimeEquals` (جلوگیری از timing attack).

## گام ۶ — اجرا
```bash
cd src/PasswordSecurityApp
dotnet build
dotnet run
```

## نکته
PasswordHasher از ASP.NET Core Identity هم قابل استفاده است، اما PBKDF2 بدون وابستگی اضافی نیاز آزمون را برآورده می‌کند.
