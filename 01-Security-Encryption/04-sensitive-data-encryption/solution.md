# راه‌حل: رفع آسیب‌پذیری AES

## گام ۱ — مشکلات کد اولیه
| مشکل | خطر |
|------|-----|
| کلید ثابت در کد | افشا با decompile |
| ECB mode | الگوی داده قابل تحلیل |
| IV ثابت/نداشتن IV | کاهش entropy |

## گام ۲ — جایگزینی با AES-GCM
- کلید از `SENSITIVE_DATA_KEY` (env) یا تولید runtime
- Nonce تصادفی ۱۲ بایتی per encryption
- Tag ۱۶ بایتی برای یکپارچگی

## گام ۳ — ساختار payload
```
[12-byte Nonce][16-byte Tag][Ciphertext]
```
همه در فایل `sensitive-data.bin` ذخیره می‌شود.

## گام ۴ — پاک‌سازی حافظه
بافر plain-text پس از رمزنگاری/رمزگشایی zero می‌شود.

## گام ۵ — جریان برنامه
1. کاربر داده حساس وارد می‌کند.
2. AES-GCM با nonce جدید رمز می‌کند.
3. blob در فایل ذخیره می‌شود.
4. برای خواندن، blob بارگذاری و decrypt می‌شود.

## گام ۶ — اجرا
```powershell
$env:SENSITIVE_DATA_KEY = "<کلید-base64-32byte>"
cd src/SensitiveDataEncryptionApp
dotnet build
dotnet run
```

## مقایسه ECB vs GCM
ECB بلوک‌های یکسان را یکسان رمز می‌کند. GCM با nonce تصادفی هر بار خروجی متفاوت تولید می‌کند و tampering را detect می‌کند.
