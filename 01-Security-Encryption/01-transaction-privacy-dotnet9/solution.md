# راه‌حل: حفظ حریم خصوصی تراکنش‌ها

## گام ۱ — دریافت کلید از محیط اجرا
کلید ۲۵۶ بیتی از متغیر محیطی `TRANSACTION_ENCRYPTION_KEY` (Base64) خوانده می‌شود. اگر کلید در کد یا فایل پروژه باشد، کارمندان داخلی می‌توانند به آن دسترسی پیدا کنند.

```powershell
$bytes = New-Object byte[] 32
[Security.Cryptography.RandomNumberGenerator]::Fill($bytes)
[Convert]::ToBase64String($bytes)
$env:TRANSACTION_ENCRYPTION_KEY = "<خروجی>"
```

## گام ۲ — پیاده‌سازی AES-GCM
- برای هر رمزنگاری یک **Nonce** تصادفی ۱۲ بایتی تولید می‌شود.
- خروجی به صورت `Nonce + Tag + Ciphertext` ذخیره می‌شود.
- پس از رمزنگاری/رمزگشایی، بافر plain-text با `CryptographicOperations.ZeroMemory` پاک می‌شود.

## گام ۳ — ذخیره فقط ciphertext در SQLite
جدول `Transactions` فقط ستون `EncryptedPayload` از نوع `BLOB` دارد. هیچ فیلد plain-text در دیتابیس وجود ندارد.

## گام ۴ — جریان برنامه
1. کاربر متن تراکنش را وارد می‌کند.
2. متن بلافاصله رمز می‌شود.
3. فقط بایت‌های رمزشده در SQLite ذخیره می‌شوند.
4. برای مشاهده، ciphertext از DB خوانده و فقط در لحظه نمایش رمزگشایی می‌شود.

## گام ۵ — اجرا
```bash
cd src/TransactionPrivacyApp
dotnet build
dotnet run
```

## نکات امنیتی
- AES-GCM هم محرمانگی و هم یکپارچگی را تضمین می‌کند.
- Nonce تصادفی برای هر عملیات از حملات replay و الگوی ECB جلوگیری می‌کند.
- کلید فقط در حافظه process و از env بارگذاری می‌شود.
