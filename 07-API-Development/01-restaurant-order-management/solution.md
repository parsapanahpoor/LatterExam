<div dir="rtl">

# راه‌حل: مدیریت سفارش رستوران

## گام ۱ — ساختار پروژه

```
RestaurantOrderApi/
├── Models/         Order, OrderStatus
├── DTOs/           درخواست/پاسخ API
├── Repositories/   IOrderRepository + InMemoryOrderRepository
├── Services/       OrderService (منطق کسب‌وکار)
├── Controllers/    OrdersController
└── Program.cs
```

## گام ۲ — وضعیت‌های سفارش

```csharp
public enum OrderStatus
{
    Pending, Preparing, Ready, Delivered
}
```

انتقال مجاز:
- `Pending` → `Preparing`
- `Preparing` → `Ready`
- `Ready` → `Delivered`

## گام ۳ — endpointها

| Method | Path | توضیح |
|--------|------|-------|
| POST | `/api/orders` | ایجاد سفارش |
| GET | `/api/orders` | لیست همه |
| GET | `/api/orders/{id}` | دریافت یک سفارش |
| PATCH | `/api/orders/{id}/status` | به‌روزرسانی وضعیت |

## گام ۴ — build و اجرا

```bash
cd src
dotnet build RestaurantOrder.sln
dotnet run --project RestaurantOrderApi
```

## گام ۵ — تست نمونه

```bash
# ایجاد سفارش
curl -X POST http://localhost:5000/api/orders \
  -H "Content-Type: application/json" \
  -d "{\"customerName\":\"علی\",\"items\":[\"پیتزا\",\"نوشابه\"],\"totalAmount\":150000}"

# لیست
curl http://localhost:5000/api/orders

# تغییر وضعیت
curl -X PATCH http://localhost:5000/api/orders/{id}/status \
  -H "Content-Type: application/json" \
  -d "{\"status\":\"Preparing\"}"
```

## نکات Clean Code

- Controller فقط HTTP را مدیریت می‌کند.
- Service اعتبارسنجی و قوانین انتقال وضعیت را اعمال می‌کند.
- Repository دسترسی به داده را جدا نگه می‌دارد.

</div>
