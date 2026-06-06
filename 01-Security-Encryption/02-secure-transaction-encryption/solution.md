<div dir="rtl">

# راه‌حل: رمزگذاری امن تراکنش در سطح برنامه

## گام ۱ — طراحی مدل داده
هر تراکنش شامل `AccountNumber`، `Amount`، `Description` و timestamp است. این فیلدها قبل از ذخیره در یک payload واحد ترکیب و رمز می‌شوند.

## گام ۲ — مدیریت کلید
- کلید AES از env `SECURE_TRANSACTION_KEY` (Base64) خوانده می‌شود.
- کلاس `SecureKeyProvider` کلید را در حافظه نگه می‌دارد و در پایان با `ZeroMemory` پاک می‌کند.
- کلید هرگز در لاگ یا فایل ذخیره نمی‌شود.

## گام ۳ — AES-GCM با IV تصادفی
- Nonce ۱۲ بایتی برای هر تراکنش تولید می‌شود.
- خروجی: `Nonce | Tag | Ciphertext`
- حتی با دسترسی به DB، فقط blob غیرقابل خواندن دیده می‌شود.

## گام ۴ — لایه Repository
`SecureTransactionRepository` فقط `EncryptedPayload` را در SQLite ذخیره می‌کند. هیچ ستون plain-text وجود ندارد.

## گام ۵ — جلوگیری از افشا در لاگ
- هیچ `Console.WriteLine` از داده plain-text قبل از رمزنگاری انجام نمی‌شود.
- پیام‌های خروجی فقط id تراکنش یا وضعیت عملیات را نشان می‌دهند.

## گام ۶ — اجرا
```powershell
$env:SECURE_TRANSACTION_KEY = "<کلید-base64>"
cd src/SecureTransactionApp
dotnet build
dotnet run
```

## چرا E2E در سطح اپلیکیشن؟
DBA با دسترسی مستقیم به SQLite فقط ciphertext می‌بیند. بدون کلید runtime (که در env است)، رمزگشایی ممکن نیست.

</div>
