# راه‌حل: سامانه نظرسنجی آنلاین

## گام ۱ — طراحی لایه‌ها (Clean Architecture)

| لایه | مسئولیت |
|------|---------|
| **Domain** | موجودیت‌های `Poll`، `PollOption`، `Vote` و enum نقش‌ها |
| **Application** | DTOها، interfaceها و `PollService` (منطق کسب‌وکار) |
| **Infrastructure** | Repository در حافظه، سرویس رمزنگاری AES |
| **Api** | Controllerها، JWT، Program.cs |

## گام ۲ — JWT و نقش‌ها

1. endpoint `POST /api/auth/token` توکن JWT صادر می‌کند.
2. نقش‌ها در claim `Role` ذخیره می‌شوند: `Admin`، `User`، `Analyst`.
3. `[Authorize(Roles = ...)]` روی actionها اعمال شده است.

## گام ۳ — رمزنگاری رأی‌دهنده

- `AesEncryptionService` شناسه کاربر را قبل از ذخیره رمز می‌کند.
- جلوگیری از رأی تکراری با مقایسه `EncryptedVoterId`.

## گام ۴ — endpointها

| Method | Path | نقش |
|--------|------|-----|
| POST | `/api/auth/token` | عمومی |
| POST | `/api/polls` | Admin |
| GET | `/api/polls` | احراز هویت‌شده |
| POST | `/api/polls/{id}/vote` | User |
| GET | `/api/polls/{id}/results` | Analyst, Admin |

## گام ۵ — build و اجرا

```bash
cd src
dotnet build PollingSystem.sln
dotnet run --project PollingSystem.Api
```

## گام ۶ — تست نمونه

```bash
# 1. دریافت توکن Admin
curl -X POST http://localhost:5000/api/auth/token \
  -H "Content-Type: application/json" \
  -d "{\"userId\":\"admin1\",\"role\":\"Admin\"}"

# 2. ایجاد نظرسنجی
curl -X POST http://localhost:5000/api/polls \
  -H "Authorization: Bearer <TOKEN>" \
  -H "Content-Type: application/json" \
  -d "{\"question\":\"بهترین زبان؟\",\"options\":[\"C#\",\"Python\",\"Go\"]}"

# 3. رأی User
curl -X POST http://localhost:5000/api/polls/{id}/vote \
  -H "Authorization: Bearer <USER_TOKEN>" \
  -d "{\"optionId\":\"<OPTION_ID>\"}"

# 4. نتایج Analyst
curl http://localhost:5000/api/polls/{id}/results \
  -H "Authorization: Bearer <ANALYST_TOKEN>"
```

## نکات طراحی

- Repository در حافظه برای demo — در production از SQL/Redis استفاده شود.
- SignalR می‌تواند برای real-time واقعی به endpoint نتایج اضافه شود.
- کلید AES و JWT secret باید از configuration امن خوانده شوند.
