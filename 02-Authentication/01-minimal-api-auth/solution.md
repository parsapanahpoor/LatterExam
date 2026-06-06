# راه‌حل: احراز هویت Minimal API

## گام ۱ — endpoint ورود
`POST /login` با body JSON:
```json
{ "username": "admin", "password": "P@ssw0rd!" }
```
در صورت صحت، token برمی‌گردد:
```json
{ "token": "secure-token-12345" }
```
در غیر این صورت **401 Unauthorized**.

## گام ۲ — محافظت endpointها
برای `/protected` و `/profile`:
1. هدر `Authorization` خوانده می‌شود.
2. باید با `Bearer ` شروع شود.
3. token استخراج و با مقدار معتبر مقایسه می‌شود.
4. در صورت خطا → **401**.

## گام ۳ — تابع کمکی
```csharp
static bool TryGetBearerToken(HttpContext context, out string token)
```
منطق استخراج token در یک نقطه متمرکز شده تا تکرار نشود.

## گام ۴ — تست
```bash
cd src/MinimalApiAuthApp
dotnet build
dotnet run
```

**ورود:**
```bash
curl -X POST http://localhost:5000/login -H "Content-Type: application/json" -d "{\"username\":\"admin\",\"password\":\"P@ssw0rd!\"}"
```

**دسترسی محافظت‌شده:**
```bash
curl http://localhost:5000/protected -H "Authorization: Bearer secure-token-12345"
```

**بدون token (باید 401 برگردد):**
```bash
curl -i http://localhost:5000/protected
```

## نکته
در production از JWT و secret management استفاده می‌شود؛ برای این چالش token ثابت کافی است.
