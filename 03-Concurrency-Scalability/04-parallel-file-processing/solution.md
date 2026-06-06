<div dir="rtl">

# راه‌حل: پردازش موازی فایل‌ها

## گام ۱ — ایجاد پوشه و فایل‌های تست
```csharp
var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles");
if (!Directory.Exists(directoryPath))
{
    Directory.CreateDirectory(directoryPath);
    for (int i = 0; i < 20; i++)
    {
        var path = Path.Combine(directoryPath, $"File{i}.txt");
        await File.WriteAllTextAsync(path, $"Initial content {i}");
    }
}
```

## گام ۲ — ساخت آرایه Taskها
```csharp
var filePaths = Directory.GetFiles(directoryPath, "*.txt");
var tasks = new Task[filePaths.Length];
for (int i = 0; i < filePaths.Length; i++)
{
    tasks[i] = ProcessFileAsync(filePaths[i]);
}
```

## گام ۳ — اجرای موازی با Task.WhenAll
```csharp
await Task.WhenAll(tasks);
```

هر فایل در Task جداگانه پردازش می‌شود و I/O به‌صورت async انجام می‌گیرد.

## گام ۴ — پردازش هر فایل
```csharp
static async Task ProcessFileAsync(string filePath)
{
    var content = await File.ReadAllTextAsync(filePath);
    var processed = content.ToUpperInvariant();
    await File.WriteAllTextAsync(filePath, processed);
}
```

## گام ۵ — اجرا
```bash
cd src/ParallelFileProcessingApp
dotnet build
dotnet run
```

محتوای `TestFiles/File0.txt` باید `INITIAL CONTENT 0` باشد.

## نکات
- پردازش موازی فایل‌ها برای I/O-bound کارها بسیار مؤثر است.
- هر فایل Task مستقل دارد؛ خطا در یک فایل را می‌توان با try-catch جداگانه مدیریت کرد.

</div>
