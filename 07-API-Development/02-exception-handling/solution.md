# راه‌حل: مدیریت Exception و پردازش ورودی

## گام ۱ — مشکل

استفاده از `int.Parse` یا `Convert.ToInt32` روی ورودی نامعتبر باعث `FormatException` و crash می‌شود.

## گام ۲ — راه‌حل: int.TryParse

```csharp
if (int.TryParse(input, out var number))
{
    sum += number;  // ورودی معتبر
}
else
{
    logger.LogError("ورودی نامعتبر: '{Input}'", input);  // رد + لاگ
}
```

## گام ۳ — ساختار

```
ExceptionHandlingDemo/
├── NumberParser.cs   منطق parse و جمع
└── Program.cs        ورودی نمونه + اجرا
```

## گام ۴ — build و اجرا

```bash
cd src
dotnet build ExceptionHandlingDemo.sln
dotnet run --project ExceptionHandlingDemo
```

## گام ۵ — خروجی مورد انتظار

ورودی: `["10", "abc", "25", "", "3.5", "7"]`

| ورودی | نتیجه |
|-------|-------|
| `"10"` | ✅ +10 |
| `"abc"` | ❌ لاگ خطا |
| `"25"` | ✅ +25 |
| `""` | ❌ لاگ خطا |
| `"3.5"` | ❌ لاگ خطا (اعشاری) |
| `"7"` | ✅ +7 |

**جمع نهایی: 42**

## نکات

- `TryParse` exception پرتاب نمی‌کند — مناسب loop روی ورودی ناشناخته.
- از `ILogger` برای لاگ ساخت‌یافته استفاده شده است.
- در production می‌توان invalid entries را در فایل یا DB ذخیره کرد.
