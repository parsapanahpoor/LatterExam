<div dir="rtl">

# راه‌حل: اصلاح Dependency Injection

## گام ۱ — شناسایی مشکل

Lifetime اشتباه (Singleton برای هر دو):

```csharp
services.AddSingleton<IRepository, Repository>();
services.AddSingleton<IService, Service>();
```

**نتیجه:** همه سرویس‌ها یک Repository مشترک دارند و `InstanceId` یکسان است.

## گام ۲ — Lifetime در DI

| Lifetime | رفتار |
|----------|-------|
| **Singleton** | یک instance برای کل برنامه |
| **Scoped** | یک instance per scope (مثلاً per request) |
| **Transient** | instance جدید در هر resolve |

## گام ۳ — اصلاح

```csharp
services.AddTransient<IRepository, Repository>();
services.AddTransient<IService, Service>();
```

- `Repository` → **Transient** (یا Scoped در Web API)
- `Service` → **Transient** تا هر resolve، Repository جدید بگیرد

## گام ۴ — ساختار پروژه

```
DependencyInjectionDemo/
├── Interfaces/     IRepository, IService
├── Repositories/   Repository (InstanceId یکتا)
├── Services/       Service (وابسته به IRepository)
└── Program.cs      ثبت DI + نمایش خروجی
```

## گام ۵ — build و اجرا

```bash
cd src
dotnet build DependencyInjectionDemo.sln
dotnet run --project DependencyInjectionDemo
```

## گام ۶ — خروجی مورد انتظار

```
Scope 1 - Service A: Service[...] -> Repository[...]
Scope 1 - Service B: Service[...] -> Repository[...]
Repository مشترک؟ False

Scope 2 - Service: Service[...] -> Repository[...]
```

`Repository مشترک؟ False` یعنی lifetime درست است.

## نکته

در Web API معمولاً `Repository` را **Scoped** و `Service` را **Scoped** یا **Transient** ثبت می‌کنند تا در هر request state جدا داشته باشند.

</div>
