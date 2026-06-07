<div dir="rtl">

# سوالات و پاسخ‌ها — C# / .NET / معماری / DevOps

> پاسخ هر سوال بین ۵ تا ۱۰ جمله است. سوالات تکراری ادغام شده‌اند.

---

## فهرست مطالب

1. [ASP.NET Core](#aspnet-core)
2. [C#](#c)
3. [Entity Framework و SQL Server](#entity-framework-و-sql-server)
4. [معماری نرم‌افزار](#معماری-نرمافزار)
5. [امنیت](#امنیت)
6. [Performance و Caching](#performance-و-caching)
7. [UI / UX](#ui--ux)
8. [Git و DevOps](#git-و-devops)
9. [Testing](#testing)

---

## ASP.NET Core

### چه زمانی باید از `IHostedService` / `BackgroundService` استفاده کرد؟

`IHostedService` برای اجرای کارهای پس‌زمینه‌ای مناسب است که باید همزمان با چرخهٔ عمر اپلیکیشن شروع و متوقف شوند؛ مثل پردازش صف، همگام‌سازی دوره‌ای، یا warmup. `BackgroundService` پیاده‌سازی آمادهٔ `IHostedService` است و معمولاً برای loopهای طولانی‌مدت با `ExecuteAsync` استفاده می‌شود. تفاوت اصلی: `IHostedService` قرارداد پایه است؛ `BackgroundService` کلاس abstract با الگوی استاندارد cancelation است. برای کارهای کوتاه یک‌باره `IHostedService` مستقیم کافی است؛ برای jobهای مداوم `BackgroundService` خواناتر است. هر دو با DI یکپارچه‌اند و باید cancellation token را رعایت کنند تا shutdown تمیز باشد. از آن‌ها برای کارهای blocking سنگین بدون offload مناسب استفاده نکنید؛ در آن صورت صف + worker بهتر است.

### تفاوت `BackgroundService` و `IHostedService` چیست؟

`IHostedService` اینترفیس با دو متد `StartAsync` و `StopAsync` است و حداقل قرارداد host برای background work را تعریف می‌کند. `BackgroundService` از `IHostedService` ارث می‌برد و متد abstract `ExecuteAsync` دارد تا منطق اصلی را در آن بنویسید. در عمل تقریباً همیشه `BackgroundService` برای jobهای تکراری انتخاب می‌شود چون boilerplate کمتری دارد. هر دو توسط `IHost` مدیریت می‌شوند و با shutdown اپ gracefully متوقف می‌شوند. برای چند worker می‌توان چند `IHostedService` ثبت کرد. انتخاب بین آن‌ها بیشتر مسئلهٔ readability است تا capability.

### `app.Use` و `app.Run` در Middleware چه تفاوتی دارند؟

`app.Use` middleware را به pipeline اضافه می‌کند و کنترل را به middleware بعدی می‌سپارد؛ یعنی باید `next()` را صدا بزنید (یا معادل آن). `app.Run` terminal middleware است؛ پس از اجرا pipeline متوقف می‌شود و درخواست‌های بعدی به middlewareهای بعد از آن نمی‌رسند. `Use` برای logging، auth، exception handling و… مناسب است. `Run` معمولاً برای fallback، health ساده، یا پاسخ ثابت انتهای pipeline استفاده می‌شود. ترتیب ثبت middleware در ASP.NET Core بسیار مهم است چون request از بالا به پایین و response از پایین به بالا عبور می‌کند.

### Custom Middleware در ASP.NET Core چگونه طراحی می‌شود؟

کلاس middleware باید `RequestDelegate _next` بگیرد و متد `Invoke` یا `InvokeAsync` داشته باشد. در `InvokeAsync` قبل و بعد از `await _next(context)` منطق خود را اجرا می‌کنید. برای DI، dependencyها را در constructor inject کنید (scoped/transient بسته به نیاز). در `Program.cs` با `app.UseMiddleware<T>()` یا extension method ثبت کنید. middleware باید سبک باشد؛ کار سنگین را به service بسپارید. exceptionها را یا handle کنید یا به middleware اختصاصی exception بسپارید. برای cross-cutting concerns مثل correlation ID، timing، و auth header parsing ایده‌آل است.

### CORS در ASP.NET Core چگونه مدیریت می‌شود؟

در `Program.cs` با `builder.Services.AddCors` policy تعریف کنید (origin، method، header، credential). سپس `app.UseCors("PolicyName")` را **قبل از** auth و endpoint mapping (و بعد از routing در نسخه‌های جدید طبق docs) قرار دهید. برای development می‌توان origin باز گذاشت؛ در production origin را whitelist کنید. `AllowCredentials` فقط با origin مشخص مجاز است نه `*`. برای API + SPA، origin فرانت را دقیق تنظیم کنید. preflight OPTIONS را policy پوشش می‌دهد. CORS جایگزین auth نیست؛ فقط مرورگر را محدود می‌کند.

### راه‌حل مدیریت Exception در ASP.NET Core

از `IExceptionHandler` / middleware سراسری (`UseExceptionHandler`) برای catch یکپارچه استفاده کنید. در development جزئیات خطا؛ در production پاسخ استاندارد بدون stack trace. `ProblemDetails` (RFC 7807) برای API یکنواخت عالی است. exceptionهای business را از technical جدا کنید (status code مناسب). logging با correlation ID انجام دهید. در Minimal API از `AddProblemDetails` و handler سفارشی بهره ببرید. validation exceptionها را به 400 با جزئیات فیلد map کنید. هرگز exception را swallow نکنید مگر با logging عمدی.

### ساختار Middleware ماژولار و قابل توسعه در پروژه بزرگ

هر concern را در کلاس جدا + extension method `UseXxx()` قرار دهید. ترتیب pipeline را در یک کلاس `MiddlewarePipelineExtensions` متمرکز کنید. configuration را با options pattern (`IOptions<T>`) inject کنید. middlewareهای اختیاری را با feature flag فعال کنید. از branching middleware (`Map`, `MapWhen`) برای مسیرهای مختلف استفاده کنید. تست unit با `DefaultHttpContext` برای middlewareهای حیاتی بنویسید. مستند کنید هر middleware چه چیزی را تضمین می‌کند. از god-middleware که همه کار را انجام می‌دهد پرهیز کنید.

### Logging پیشرفته، قابل گسترش و سفارشی در ASP.NET Core

از `ILogger<T>` و providerهای استاندارد (Console، Debug، EventSource) شروع کنید. Serilog یا NLog برای sinkهای غنی (Seq، Elasticsearch، file) مناسب‌اند. structured logging با placeholder (`LogInformation("Order {OrderId}", id)`) بجای string concat. scope با `BeginScope` برای correlation و tenant. filter و minimum level per namespace در config. enricher برای machine name، environment، user. در production PII را log نکنید. OpenTelemetry برای یکپارچگی trace + log + metric در سیستم بزرگ.

### چگونه Controller در ASP.NET Core ثبت و استفاده می‌شود؟

با `builder.Services.AddControllers()` و `app.MapControllers()` در minimal hosting. controller کلاس با `[ApiController]` و `[Route]`؛ actionها با HTTP attribute. DI در constructor controller انجام می‌شود. convention routing یا attribute routing. برای API فقط JSON، `AddControllers()` کافی است؛ برای MVC views از `AddControllersWithViews()` استفاده کنید. validation خودکار با `[ApiController]` و DataAnnotations/`FluentValidation`. controller باید thin باشد؛ logic در service layer.

### مراحل پیاده‌سازی احراز هویت JWT در ASP.NET Core

کاربر login می‌کند؛ credential validate؛ JWT با claims (sub، role، exp) sign با secret/asymmetric key صادر می‌شود. `AddAuthentication(JwtBearerDefaults...)`. `AddJwtBearer` با `TokenValidationParameters` (issuer، audience، signing key). `[Authorize]` روی endpointها. refresh token اختیاری برای UX بهتر. secret را در Key Vault/User Secrets نگه دارید. HTTPS اجباری. access token کوتاه‌مدت؛ refresh بلندمدتر با rotation.

### Rate Limiting در ASP.NET Core — راهکارها

از middleware built-in .NET 7+ (`AddRateLimiter`) با policy fixed/sliding window استفاده کنید. partition بر اساس IP، user ID، یا API key. `429 Too Many Requests` با Retry-After. برای distributed scenario Redis-based limiter (AspNetCoreRateLimit یا custom). ترکیب با API Gateway (Azure APIM، nginx). whitelist برای health/internal. log hit rate برای tuning. rate limit را در load test validate کنید.

### `IActionResult` در مقابل `ActionResult<T>` — تفاوت

`IActionResult` نوع بازگشتی عمومی برای Ok، BadRequest، NotFound و… است بدون type-safe body. `ActionResult<T>` هم HTTP result می‌دهد هم typed value برای OpenAPI/Swagger و compile-time safety. برای API جدید `ActionResult<T>` ترجیح دارد چون contract واضح‌تر است. هر دو با `[ProducesResponseType]` مستند می‌شوند. `IActionResult` وقتی چند نوع پاسخ متفاوت دارید انعطاف بیشتری دارد.

### چه زمانی SignalR توصیه می‌شود؟

وقتی نیاز به push real-time از server به client دارید: chat، notification، live dashboard، progress. WebSocket با fallback long polling. scale-out با Redis backplane یا Azure SignalR Service. برای simple REST polling کافی است SignalR اضافه است. auth hub با `[Authorize]` و group management. connection lifecycle و reconnect را در نظر بگیرید.

### مدیریت Configuration در چند محیط (Dev/Staging/Production)

`appsettings.json` پایه + `appsettings.{Environment}.json` override. متغیر محیطی با prefix `ASPNETCORE_` و `__` برای nested. User Secrets فقط development. Azure App Configuration / Key Vault برای production secrets. `IOptions<T>` / `IOptionsMonitor<T>` برای bind.section. هر environment connection string جدا. از hardcode secret در repo خودداری کنید. validate options at startup با `ValidateOnStart`.

### Razor Pages در مقابل MVC — مزایا

Razor Pages برای UI page-centric ساده‌تر است؛ هر صفحه model + handler خودش (`OnGet`, `OnPost`). MVC برای app با separation قوی controller/action مناسب‌تر است. Razor Pages سریع‌تر scaffold می‌شود و coupling کمتری در پروژه‌های کوچک دارد. routing convention-based (`/Products/Edit`). برای SPA backend معمولاً API Controller کافی است. انتخاب بسته به team skill و ساختار UI است.

### Localization در ASP.NET Core — مراحل

`AddLocalization` + `RequestLocalizationOptions` (culture providers: query، cookie، Accept-Language). resource files `.resx` per culture. `IStringLocalizer<T>` در controller/view inject. `DataAnnotations` localization با adapter. RTL/LTR و format تاریخ/عدد per culture. fallback culture تعریف کنید. cache resource در production.

### API Versioning — راهکار، مزایا و معایب

روش‌ها: URL (`/v1/`)، header (`api-version`)، query string. package `Asp.Versioning.Mvc` یا convention custom. مزایا: breaking change بدون قطع client قدیمی. معایب: پیچیدگی maintenance، duplicate controller، test بیشتر. sunset policy برای نسخه‌های قدیمی. OpenAPI per version. default version برای backward compatibility.

### Clean Architecture در ASP.NET Core — پیاده‌سازی

لایه‌ها: Domain (entities، rules) → Application (use cases، interfaces) → Infrastructure (EF، external) → Presentation (API). وابستگی فقط به داخل؛ Application به Domain، Infrastructure به Application. DI در Infrastructure ثبت implementationها. controller فقط use case را صدا می‌زند. mapping با AutoMapper یا manual. test Application بدون DB با mock repository. جدا کردن پروژه‌ها در solution برای enforce boundaries.

### Cross-cutting concerns (Log، Cache) بهینه در پروژه چندلایه

Logging: middleware + `ILogger` در service؛ correlation ID. Cache: `IDistributedCache` abstraction در Application؛ Redis implementation در Infrastructure. Decorator pattern روی service برای cache-aside. policy expiration per data type. از cache در Domain استفاده نکنید. Aspect-like behavior با middleware/filter برای auth و logging. configuration per environment.

### وابستگی بین لایه‌ها در Clean Architecture (DI)

Application interfaces (`IOrderRepository`) را Domain/Application تعریف می‌کند. Infrastructure پیاده‌سازی می‌کند و در composition root (`Program.cs` یا extension) register می‌شود. Presentation فقط Application را reference کند نه Infrastructure مستقیم (جز bootstrap). `AddInfrastructure(this IServiceCollection)` extension. scoped برای DbContext و repository per request. از service locator پرهیز کنید.

### DI چه زمانی مشکل / Memory Leak ایجاد می‌کند؟

Captive dependency: Singleton وابسته به Scoped (مثل DbContext در singleton). راه‌حل: lifetime درست یا `IServiceScopeFactory`. event subscription بدون unsubscribe در singleton. `IDisposable` ثبت‌شده که dispose نمی‌شود. holding reference به HttpContext در singleton. از `BuildServiceProvider` دستی در configure پرهیز. validate با DI validation at startup. hosted service باید scope جدا برای scoped service بسازد.

### درخواست‌های کند — شناسایی و رفع

Application Insights، OpenTelemetry، MiniProfiler برای trace. log slow request threshold. DB: missing index، N+1 query. async I/O واقعی؛ از sync-over-async پرهیز. connection pooling. response compression. caching برای read-heavy. horizontal scale. load test با k6/JMeter قبل از production.

---

## C#

### DI در پروژه بزرگ — چرخه عمر و کارایی

`Singleton`: یک instance برای کل app؛ مناسب stateless service؛ مراقب thread-safety و captive dependency. `Scoped`: per request (یا per scope)؛ ideal برای DbContext و unit of work. `Transient`: هر resolve جدید؛ برای lightweight stateless؛ هزینهٔ ساخت بیشتر. در ASP.NET Core ترکیب استاندارد: Singleton برای cache/config، Scoped برای business+DB، Transient برای helper سبک. `IOptions`/`IOptionsMonitor` برای config. از singleton برای mutable state پرهیز. profiling برای over-transient. document lifetime هر service.

### LINQ برای چه منظور است؟

Language Integrated Query برای query کردن collections، DB (via providers)، XML با syntax یکسان. deferred execution با `IEnumerable`؛ remote execution با `IQueryable`. خوانایی و composability بالاتر از loop. اما باید بدانید چه زمانی materialize می‌شود (`ToList`, `Count`). برای in-memory مناسب؛ برای EF ترجمه به SQL. از client-side evaluation ناخواسته پرهیز.

### کلمهٔ `async` / `await` — کاربرد

`async` متد را state machine می‌کند تا await بدون block thread. برای I/O-bound (DB، HTTP، file) thread pool را آزاد می‌کند. `await` continuation بعد از complete. `ConfigureAwait(false)` در library code. CPU-bound را با `Task.Run` جدا کنید نه fake async. async void فقط event handler. deadlock با `.Result`/`.Wait()` در UI/sync context. throughput بهتر در server با async I/O.

### `var` — کاربرد

type inference سمت راست assignment. code کوتاه‌تر وقتی type واضح است (`var list = new List<int>()`). از `var` وقتی type مبهم است (`GetData()`) پرهیز برای readability. در anonymous type مجبور به var. style guide team را follow کنید.

### `delegate` — چه زمانی ضروری است؟

callback، event، LINQ (`Func`, `Action`). وقتی behavior را as parameter pass می‌کنید. custom equality/comparison. legacy API interop. در C# مدرن often `Func`/`Action`/`Predicate` کافی است. multicast delegate برای event. understanding delegate برای event memory leak مهم است.

### پلاگین‌پذیری با ویژگی‌های C#

`Assembly.LoadFrom` + reflection discover `IPlugin`. MEF یا `IServiceCollection` dynamic registration. convention-based scanning (`AddPlugins()`). isolate plugin در AppDomain جدا (legacy) یا process جدا برای security. version conflict با `AssemblyLoadContext` در .NET Core+. contract stable (`IPlugin`) در core assembly. sandbox و permission برای untrusted plugin.

### Thread-safe بودن در multi-threaded

`lock` برای critical section کوتاه. `ConcurrentDictionary/Bag/Queue` برای collection مشترک. `Interlocked` برای counter ساده. immutable data structures where possible. avoid lock hierarchy deadlock. `ReaderWriterLockSlim` برای read-heavy. async code از shared mutable state بدون sync پرهیز. `SemaphoreSlim` برای throttle.

### `struct` در مقابل `class`

struct value type روی stack (معمولاً)؛ copy semantics؛ مناسب data کوچک immutable (`Point`, `DateRange`). class reference type؛ heap؛ inheritance. struct بزرگ (>16 bytes rule of thumb) copy costly. nullable struct `struct?`. برای DDD entity معمولاً class؛ Value Object کوچک می‌تواند struct record.

### Lambda Expression — مزایا

syntax فشرده برای anonymous function. closure over local variables. LINQ، event handler، callback. expression tree برای EF translation. capture variable reference — مراقب loop closure bug. local function sometimes readableتر.

### Singleton Thread-Safe در C#

double-checked locking با `Lazy<T>` (`LazyThreadSafetyMode.ExecutionAndPublication`) preferred. static nested class (holder) lazy init. avoid naive check-then-act without lock. در DI معمولاً container singleton می‌سازد — manual singleton کمتر نیاز. `Interlocked.CompareExchange` برای advanced case.

### Reflection — سناریو و ریسک

plugin discovery، serialization framework، ORM mapping، test mock. مشکل: performance overhead، break با refactor، security (invoke private). cache `MethodInfo`/`PropertyInfo`. source generator (.NET 5+) جایگزین compile-time. minimize reflection در hot path. restrict در trusted code only.

### Event — چه زمانی مشکل ایجاد می‌کند؟

subscriber reference publisher/subscriber را زنده نگه می‌دارد → memory leak. static event خطرناک‌تر. همیشه `-=` unsubscribe در Dispose. weak event pattern برای rare case. async void event handler exception handling. long-running handler block publisher.

### Event Sourcing با C#

state = sequence of events؛ rebuild با replay. audit trail عالی. با MediatR + event store (EventStoreDB، SQL). snapshot برای performance. eventual consistency. complexity بالا؛ برای domain با audit/complex workflow مناسب. projection برای read model. versioning event schema.

### Pattern Matching — مزایا و سنario

`switch` expression، `is` pattern، `record` deconstruct. خوانایی بهتر از cascade if/type check. exhaustiveness check compiler. برای domain rule و parsing JSON/tree. performance comparable. C# 11+ list patterns.

### Generics برای کاهش وابستگی در سیستم ماژولار

repository `<T>` generic، handler `<TCommand>`. constraint `where T : class`. open generic registration DI. less duplicate code. avoid generic overuse obscuring intent. covariance/contravariance `in`/`out` در interface.

### ValueTuple در مقابل class

lightweight multi-return بدون define type. named fields `(int Id, string Name)`. stack allocated often. برای private helper return مناسب؛ public API contract بهتر named record/class. comparison value semantics.

### Memory Leak جلوگیری — مثال

event unsubscribe. dispose `IDisposable` (`using`). avoid static collection holding objects. `HttpClient` via `IHttpClientFactory` not new per call. timer callback holding reference. example: `EventSubscriber` implements `IDisposable` and `-= handler`.

### Nullable Reference Types (C# 8+)

`string?` vs `string` compile warning. reduce NullReferenceException. `#nullable enable`. require explicit null check or `!`. migration gradual. `[NotNullWhen]` attributes. better API contract.

### Task در مقابل Thread

Thread OS-level؛ expensive create. Task abstraction؛ often thread pool. `Task.Run` offload CPU work. async Task for I/O. `Thread` برای dedicated long-running یا STA legacy. prefer Task/TPL in modern code. `Parallel.ForEach` for CPU parallel.

### async/await — چه زمانی بهتر / بدتر

بهتر: I/O DB/network/file. بدتر: pure CPU tight loop (use Parallel). fake async (`Task.Run` wrap sync) anti-pattern in ASP.NET. UI: await keeps responsive. library: `ConfigureAwait(false)`. measure under load.

### LINQ در مقابل حلقه سنتی

LINQ declarative؛ composable. loop imperative؛ sometimes faster micro-optimization. LINQ to Objects overhead negligible often. EF: LINQ translates SQL — wrong query costly. `foreach` clearer for side-effect only. benchmark hot path.

### LINQ بهینه در حجم بالا

filter early (`Where` before `Select`). push work to DB (`IQueryable` not `AsEnumerable` early). projection `Select` not full entity. avoid multiple enumeration — materialize once. compiled query EF. streaming `IAsyncEnumerable`. index DB columns used in Where.

### Parallel.ForEach در مقابل Task.WhenAll

`Parallel.ForEach` sync CPU parallel with partitioner؛ blocks calling thread until done. `Task.WhenAll` async tasks concurrent I/O. mix: Parallel for CPU batch، WhenAll for async IO list. don't Parallel.ForEach async lambda wrongly. degree of parallelism control.

### IEnumerable در مقابل IQueryable

`IEnumerable` in-memory enumeration (LINQ to Objects). `IQueryable` expression tree + provider (EF to SQL). `IQueryable` deferred remote execution. calling `ToList()` too early pulls all data. understand provider boundary. AsNoTracking read-only EF.

### ValueTask در مقابل Task

`ValueTask` avoid allocation when result often synchronous cached. for hot path micro-optimization. must consume once. library code pattern. default API still `Task` for simplicity. don't store ValueTask field.

### `Span<T>` — زمان و محدودیت

stack-only ref struct؛ slice array/memory without copy. parsing، crypto، high perf. cannot box، async، field on heap class limited. `ReadOnlySpan` for input. `Memory<T>` for async scenario. unsafe advanced interop.

### آرایه — تعریف و مقداردهی

`int[] arr = new int[5];` یا `int[] arr = { 1, 2, 3 };`. jagged `int[][]`. multidimensional `int[,]`. collection expression C# 12 `[1,2,3]`. length fixed. LINQ/List flexible size.

### Interface در مقابل Abstract Class

interface contract only؛ multiple inheritance. abstract class shared implementation + state. interface for capability (`IDisposable`). abstract for base hierarchy shared code. default interface method C# 8. prefer composition over deep hierarchy.

### Singleton Thread-Safe (تکراری با DI context)

در .NET مدرن: register as Singleton in DI. manual: `Lazy<T>` or static nested. thread-safe lazy init critical. avoid lock on every access after init with double-check/Lazy.

---

## Entity Framework و SQL Server

### DbSet در مقابل DbContext

`DbContext` unit of work + session؛ configuration، change tracking، SaveChanges. `DbSet<T>` gateway to entity table/query on that context. one context multiple DbSet. context lifetime typically scoped per request. DbContext not thread-safe. DbSet = `context.Set<T>()` equivalent.

### `Include` — کاربرد

eager loading related entity (`Include(o => o.Items)`). avoid N+1 when need related data upfront. `ThenInclude` nested. over-include hurts performance — project selectively. `AsSplitQuery` multiple SQL. alternative: explicit projection Select.

### Transaction در EF — مدیریت

`await context.Database.BeginTransactionAsync()` wrap multiple SaveChanges. `Commit`/`Rollback`. ambient `TransactionScope` distributed rare. idempotent retry careful. isolation level default READ COMMITTED. short transaction reduce lock. same context one transaction.

### Migration چندتیمی — راه‌حل

single migration branch؛ one person merge migration per sprint. `--idempotent` script for DBA review. baseline snapshot. avoid parallel Add-Migration without sync. CI validate migration apply on clean DB. rename conflict resolve manually. document breaking migration.

### Migration — چه مشکلاتی

data loss column drop. long lock production. merge conflict designer snapshot. rollback hard — forward-fix preferred. test on copy production. zero-downtime deploy strategy (expand-contract).

### HasOne / HasMany در Fluent API

`HasOne(x => x.Parent).WithMany(x => x.Children).HasForeignKey(x => x.ParentId)` relationship cardinality. cascade delete configure. required/optional with `IsRequired`. shadow FK. many-to-many join entity EF Core 5+.

### Stored Procedure + EF — چه زمانی

heavy report، bulk operation، legacy DB، tuning DBA-specific. `FromSqlRaw` mapped keyless type. bypass change tracking read. maintain in DB source control. not for simple CRUD. security parameterize always.

### DDD model با EF

aggregate root = entity with consistency boundary. value object owned type / complex type / record embed. repository per aggregate not generic god repo. domain logic in entity not anemic. bounded context separate DbContext optional. avoid cross-aggregate direct navigation lazy load leak.

### Value Object در EF Core

owned entity `OwnsOne`/`OwnsMany`. immutable C# record. no identity column. equality by value. Address، Money، DateRange. table splitting optional. query limitation on owned sometimes.

### امنیت داده حساس در EF

encrypt column (Always Encrypted SQL). don't log parameter values. mask in DTO not entity expose. row-level security SQL Server. least privilege DB user. no connection string in code. audit sensitive access.

### حجم بالا — memory و performance EF

pagination `Skip/Take`. projection Select not full entity. `AsNoTracking` read. batch `ExecuteUpdate/Delete` EF7+. streaming IAsyncEnumerable. avoid Include cartesian explosion. compiled query. read replica.

### Transaction — جلوگیری از concurrency error

optimistic concurrency token `[Timestamp]`/`RowVersion`. catch `DbUpdateConcurrencyException` retry or merge. short transaction. Serializable rarely — deadlock risk. retry policy Polly. business rule validate before save.

### Primary Key در مقابل Unique Key

PK uniquely identifies row؛ one per table؛ clustered default SQL Server. Unique Key allows one NULL typically；multiple per table；can be nonclustered. PK not null. UK enforce business uniqueness (email). PK often surrogate identity.

### Clustered در مقابل Non-Clustered Index

clustered = table sort order؛ one per table. leaf = data row. nonclustered separate structure with key pointer. query filter column index. covering index include columns. wrong index hurt write. maintenance rebuild/reorganize.

### Query Performance — بهینه‌سازی SQL Server

execution plan analyze. avoid SELECT *. index WHERE/JOIN/ORDER. parameter sniffing awareness. statistics update. avoid function on indexed column in WHERE. pagination OFFSET large slow — keyset. tempdb spill investigate.

### Partitioning — حجم بالا

table partition by range (date) on partition scheme. partition elimination query perf. manage archive switch partition. sliding window. not magic — plan still matters. operational complexity.

### Deadlock — شناسایی و رفع

trace flag 1222 extended events. retry transient deadlock. consistent lock order access tables. short transaction. index reduce scan lock. `SET DEADLOCK_PRIORITY` last resort. monitor `sys.dm_tran_locks`.

### High Availability SQL Server

Always On Availability Groups failover. log shipping cheaper. replication read scale. backup restore RTO/RPO. multi-subnet cluster. test failover regularly. witness/quorum.

### Transaction Isolation Serializable — مشکل

phantom read prevented but highest lock/deadlock. throughput drop. use only when strictly needed. snapshot isolation alternative many cases. business tolerance vs correctness.

### Performance DB — حجم تراکنش بالا

index tuning، partition، archive cold data. read replica reporting. connection pool. batch insert SqlBulkCopy. monitor wait stats. hardware IO path. query store baseline.

### Distributed Transaction SQL Server

MSDTC cross-database — complexity latency. avoid if possible: saga pattern outbox. eventual consistency microservices. single DB per service preferred. two-phase commit last resort.

---

## معماری نرم‌افزار

### Monolithic در مقابل Microservices

Monolith: deploy واحد، ساده debug، coupling بالا، scale کل app. Microservices: deploy مستقل، team autonomy، network complexity، distributed transaction سخت. start monolith modular؛ split when bounded context clear. strangler fig migration.

### Microservices — Transaction و راهکار

no single ACID cross-service. saga (choreography/orchestration). outbox pattern reliable event. idempotent consumer. compensating transaction. eventual consistency accept. avoid 2PC.

### Microservices — مقیاس‌پذیری مزایا و چالش

scale service independent. tech diversity. challenge: observability، deployment، data consistency، network latency. service mesh overhead. API gateway single entry. contract testing.

### جلوگیری از coupling زیاد بین سervices

API contract versioned. async event over sync chain. anti-corruption layer. don't share DB. domain events. team per service ownership. consumer-driven contract test.

### Service Mesh — چه زمانی

many services mTLS، retry، circuit breaker، traffic split uniform. Istio/Linkerd overhead justified at scale. small team YAGNI. observability benefit. kubernetes common host.

### Clean Architecture — Interface Adapter

لایه infrastructure + presentation adapter. تبدیل external format به use case format. controller → DTO → command. repository implementation EF. isolate framework from domain.

### DDD — مدیریت پیچیدگی دامنه

bounded context، ubiquitous language. aggregate enforce invariant. context map between teams. strategic DDD before tactical. event storming discovery. don't DDD simple CRUD unnecessarily.

### Value Object در DDD — چه زمانی

no identity؛ defined by attributes. Money، EmailAddress. immutable. validate in constructor. replace not mutate. EF owned type map.

### Event Sourcing — مزیت و چallenge

complete audit، temporal query， rebuild state. challenge: schema evolution， snapshot， learning curve， eventual read model. not for every CRUD. combine with CQRS often.

### Event-Driven Architecture در .NET

MassTransit/RabbitMQ/Azure Service Bus. publish domain event after SaveChanges (outbox). consumer idempotent. dead letter queue. schema registry event version. monitor lag. MediatR in-process lighter option.

### وابستگی پیچیده در معماری لایه‌ای

dependency inversion — high level define interface. facade for legacy subsystem. anti-corruption layer. don't skip layers call. module boundary clear. integration test composition root.

### Reliability در سیستم توزیع‌شده

redundancy، health check، circuit breaker Polly， timeout， retry with backoff. idempotent operation. graceful degradation. chaos engineering test. RTO/RPO define. bulkhead isolation.

### API Reusability

stable contract، versioning، HATEOAS optional. generic pagination/filter pattern. error format standard ProblemDetails. documentation OpenAPI. don't leak internal model — DTO.

### Rate limiting / heavy request (architecture level)

queue long task background. async response 202 Accepted + polling/webhook. CDN static. API gateway throttle. horizontal pod autoscale.

---

## امنیت

### JWT امنیت — بهبود در .NET

short expiry، strong signing (RS256 asymmetric). refresh token rotation + reuse detection. store secret Key Vault. validate issuer audience clock skew. HTTPS only. don't put sensitive data in payload. HttpOnly cookie alternative XSS mitigation for token storage debate.

### JWT در API عمومی — auth و authorization

OAuth2/OIDC for third party. scope-based access. `[Authorize(Policy)]`. rate limit per client. introspection/revocation list for critical. mTLS B2B optional. audit API key rotation.

### احراز هویت چندسطحی — Token management

step-up auth sensitive action. separate token type short lived. device binding optional. refresh family invalidate all on breach. admin revoke session store.

### XSS — چیست و prevention

inject script in page. encode output HTML context. CSP header restrict script source. avoid `innerHTML` untrusted. Razor auto encode default. sanitize rich text library. HttpOnly cookie.

### CSRF — سنario و مقابله

attacker site POST to your bank transfer while user logged in. anti-forgery token synchronizer pattern. SameSite cookie Strict/Lax. verify Origin/Referer. GET idempotent no state change. double submit cookie API SPA careful.

### CSRF + XSS در پروژه چندکاربره

combine: CSP، encode， anti-forgery MVC، SameSite， CORS strict API. separate cookie auth SPA BFF pattern. security headers middleware. regular dependency scan. OWASP ASVS checklist.

### Hashing رمز عبور — چرا ساده ناامن

MD5/SHA fast brute force. use Argon2id/bcrypt/PBKDF2 with salt per user. pepper server-side secret. never reversible encrypt password. ASP.NET Identity PasswordHasher.

### Security Headers / CSP — چه زمانی

always production web. CSP against XSS. X-Frame-Options clickjacking. HSTS HTTPS. Referrer-Policy. Permissions-Policy. middleware `UseSecurityHeaders` or nginx.

---

## Performance و Caching

### Caching — مزایا

reduce latency DB/backend. higher throughput. lower cost. cache-aside pattern common. TTL appropriate staleness tolerance. invalidate on write or event.

### Memory Cache در مقابل Distributed Cache

Memory: in-process fast；single node；lost on restart. Distributed (Redis): shared multi instance；network latency；session sticky not needed. hybrid L1 memory L2 Redis. choose by scale and consistency need.

### Memory Cache — مشکل و راهکار

 stale data， memory pressure eviction unpredictable. size limit `Size` configuration. don't cache user-specific huge object unbounded. monitor memory. distributed for multi-node consistency. cache key namespace version.

### Bottleneck tools .NET

dotnet-counters، dotnet-trace， PerfView， Application Insights， JetBrains dotTrace. SQL Extended Events. load test k6. MiniProfiler dev. OpenTelemetry metrics latency p95 p99.

### Heavy Requests ASP.NET

async I/O، streaming response، pagination. background queue Hangfire. response compression. timeout cancel token. don't load huge list memory. CDN edge.

### Bottleneck scenario — مثال و رفع

symptom: p95 spike. find: SQL missing index N+1. fix: index + projection + cache read model. validate load test before/after. monitor regression CI perf gate optional.

### Monitoring کارایی پروژه متوسط

App Insights or Prometheus + Grafana. health endpoint `/health`. log structured. alert p95 error rate. dashboard RED metrics. synthetic transaction probe.

### UX — سرعت بارگذاری

lazy load image， bundle minify， CDN， HTTP/2/3， critical CSS， defer JS. server-side cache fragment. compress Brotli. reduce round trip API batch. Core Web Vitals LCP FID CLS.

---

## UI / UX

### MVC / MVVM — ساده‌تر کردن توسعه

MVC: Model-View-Controller separation web. MVVM: ViewModel binding WPF/MAUI two-way. test ViewModel without UI. clear responsibility reduce spaghetti code-behind. ASP.NET Core MVC/Razor for server render. Blazor MVVM-like component state.

### Responsive در مقابل Adaptive

Responsive: one layout fluid CSS media query flex/grid. Adaptive: distinct layout/template per breakpoint or device server detect. responsive simpler maintain. adaptive fine control large enterprise. mobile-first responsive common.

### SPA در مقابل MPA

SPA: single page client route (React/Angular)； smooth UX； SEO harder initial； JS bundle large. MPA: full page server render； SEO simple； slower navigation. choose SPA rich interactive dashboard. MPA content site form-heavy SEO. hybrid SSR Next.js.

### Accessibility — راهکار

semantic HTML، aria-label، keyboard nav tab order. color contrast WCAG. focus visible. screen reader test. form label associated. skip link. axe DevTools audit.

### UX فرم ثبت‌نام

minimal field، inline validation، clear error Persian. password strength hint. show/hide password. autocomplete attribute. progress indicator multi-step. mobile friendly touch target 44px. success feedback.

### UX عوامل بهبود وب

performance، consistency، feedback loading state， error recoverable， accessible، mobile friendly， trust signal. user research test. reduce cognitive load. clear CTA.

---

## Git و DevOps

### Merge در مقابل Rebase

Merge: preserve history branch point； merge commit. Rebase: linear history replay commits on new base； rewrite hash. merge safe shared branch. rebase clean feature branch before merge PR. never rebase public shared branch. team policy document.

### Conflict مدیریت — تیم بزرگ

small frequent merge، short-lived branch. clear code ownership CODEOWNERS. communicate schema migration lock. use merge tool VS/Rider. rebase update feature often from main. pair on hard conflict. CI catch conflict early.

### Branch Strategy — چه زمانی و انواع

team > few devs parallel release. Git Flow (release/hotfix) enterprise scheduled release. GitHub Flow simple CI main. trunk-based short branch high velocity. choose by release cadence. protect main branch.

### Branch Protection

require PR review， status check pass， no force push main. signed commit optional. required linear history optional. enforce quality gate. compliance audit.

### Branch مدیریت — پروژه بزرگ

naming convention `feature/`， `bugfix/`， `release/`. delete merged branch auto. release branch stabilize. tag semver release. monorepo path filter CI.

### CI در مقابل CD

CI: integrate code automatic build test on push — fast feedback quality. CD: deploy automatic to environment after CI pass — faster delivery risk if test weak. CI mandatory almost always. CD degree manual approval prod gate.

### CI/CD — مفهوم و مزایا

automation build test deploy. reduce human error. faster time to market. repeatable environment. rollback pipeline artifact. culture DevOps collaboration.

### Build Pipeline در مقابل Release Pipeline

Build: compile， unit test， artifact package. Release: deploy artifact to env with approval gate configuration transform. separate concern Azure DevOps/GitHub Actions stages. promote same artifact prod not rebuild.

### Deploy مکرر Zero Downtime

blue-green switch traffic. canary gradual. rolling update kubernetes. database migration backward compatible expand-contract. feature flag decouple deploy release. health check drain connection.

### Infrastructure as Code — بهبود و زمان استفاده

Terraform/Bicep/ARM versioned infra reproducible. PR review infrastructure. drift detection. environment parity dev≈prod. disaster recovery rebuild. use when infra complex multi env team scale. not for one tiny VM manual ok.

---

## Testing

### Test Coverage بالا — راهکار

pyramid: many unit، fewer integration， minimal e2e. measure coverage tool coverlet؛ target critical path not 100% blindly. mutation testing optional quality. CI fail drop threshold. test behavior not implementation detail. refactor friendly tests.

### تست سرویس خارجی

mock `HttpMessageHandler`/`WireMock.Net`. contract test Pact consumer-driven. sandbox API vendor. record/replay careful stale. integration test container Testcontainers. fallback timeout test.

### E2E — سنario اهمیت

checkout payment flow: user login → cart → pay → confirm email. multi service UI DB integration. catch regression cross team. Playwright/Selenium. run nightly not every commit slow. test data isolate.

### Integration Test — مدیریت پروژه بزرگ

Testcontainers SQL/Redis real dependency. collection fixture share container. parallel test isolate database. category trait `[Trait("Category","Integration")]`. run pipeline stage separate. cleanup after test.

### تست خودکار — سرعت و کیفیت

parallel test run. flaky test quarantine fix. test data builder pattern. avoid Thread.Sleep use await poll. CI shard test job. unit fast <100ms target. review test in PR same as code.

### Build vs Release (testing angle)

artifact immutable promote stages. test gate each stage. smoke test post deploy automated.

---

## DevOps تکمیلی

### IaC Terraform/ARM — مزیت زمانی

multi region repeat. disaster recovery template. audit change git history. cost estimate plan. team review PR infra. compliance baseline module.

---

*پایان سند — تعداد سوالات یکتا: ۱۰۰+ | تاریخ تهیه: ۱۴۰۴*

</div>
