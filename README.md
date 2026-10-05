# Idam.EntityFrameworkCore.Timestamps

[![NuGet](https://img.shields.io/nuget/v/Idam.EntityFrameworkCore.Timestamps.svg)](https://www.nuget.org/packages/Idam.EntityFrameworkCore.Timestamps)
[![Build Status](https://github.com/idamachmadfaizin/Idam.EntityFrameworkCore.Timestamps/actions/workflows/test.yml/badge.svg)](https://github.com/idamachmadfaizin/Idam.EntityFrameworkCore.Timestamps/actions)

A .NET library for entity timestamps and softdelete. Easily manage CreatedAt, UpdatedAt, and DeletedAt fields with support for DateTime, UTC DateTime, DateTimeOffset, and Unix time (milliseconds).

## :star: Support

If you find this library helpful, please consider giving it a star! Your support helps make it better.

## :rocket: Features

- Automatic handling of entity timestamps (**CreatedAt**, **UpdatedAt**, **DeletedAt**) via a **SaveChanges interceptor** — no base class, no overrides.
- Built-in **soft delete** functionality with global query filters.
- **IQueryable extension methods** to easily query soft-deleted data (`IncludeTrashed()`, `OnlyTrashed()`).
- Support for multiple timestamp formats:
  - Local `DateTime`
  - `UTC DateTime`.
  - `DateTimeOffset`, written in UTC — the recommended choice for new code.
  - `Unix Time (milliseconds)` ([learn more](https://currentmillis.com)) ([docs](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset.tounixtimemilliseconds)).
- Seamless integration with existing **EF Core**.
- Customizable field names with `[Column]` attribute.
- Flexible interfaces for individual timestamp requirements.
- Injectable clock via `TimeProvider`, for deterministic tests.

## :package: Installation

.NET CLI
```shell
dotnet add package Idam.EntityFrameworkCore.Timestamps
```

## :wrench: Basic Setup

### 1. Configure `DbContext`

Register the interceptor. It covers both `SaveChanges` and `SaveChangesAsync`, so there is
nothing to override.

```csharp
using Idam.EntityFrameworkCore.Timestamps.Extensions;

public class MyDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddTimeStampsInterceptor();
    }
}
```

Or when the options are built outside the context:

```csharp
builder.Services.AddDbContext<MyDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    options.AddTimeStampsInterceptor();
});
```

<details>
<summary>Without the interceptor</summary>

`ChangeTracker.AddTimestamps()` does the same work and can be called directly. Use this only if
you already override `SaveChanges` for other reasons — and do not combine it with the
interceptor, or the timestamps are computed twice.

```csharp
public override int SaveChanges(bool acceptAllChangesOnSuccess)
{
    ChangeTracker.AddTimestamps();

    return base.SaveChanges(acceptAllChangesOnSuccess);
}

public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
{
    ChangeTracker.AddTimestamps();

    return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
}
```

</details>

### 2. Define Your Entity 

Implement the appropriate timestamps interface (`ITimeStamps`, `ITimeStampsUtc`, `ITimeStampsOffset` or `ITimeStampsUnix`).

```csharp
using Idam.EntityFrameworkCore.Timestamps.Interfaces;

/// Local DateTime
public class Product : ITimeStamps
{
    ...
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// UTC DateTime
public class Product : ITimeStampsUtc
{
    ...
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// DateTimeOffset
public class Product : ITimeStampsOffset
{
    ...
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

/// Unix Time
public class Product : ITimeStampsUnix
{
    ...
    public long CreatedAt { get; set; }
    public long UpdatedAt { get; set; }
}
```

> [!TIP]
> **Prefer `DateTimeOffset` for new code.** The value carries its own offset, so it round-trips as
> the same instant on every provider, to the column's precision (microseconds on SQL Server and
> PostgreSQL; whole seconds on MySQL, see below). There is no `DateTimeKind` to lose, and PostgreSQL
> does not reject it. The library always writes it with **offset zero (UTC)**, because PostgreSQL's
> `timestamp with time zone` accepts nothing else. Use `.ToLocalTime()` or `.ToOffset(...)` when
> displaying it.
>
> In .NET, `DateTimeOffset` equality compares instants. In a query, the provider decides. SQLite has no
> native `DateTimeOffset` type: EF Core stores it as text, compares it as text, and cannot translate
> ordering or range comparisons. Query with UTC values, the same offset the library writes:
>
> ```csharp
> // Matches nothing on SQLite if the value carries another offset, even for the same instant.
> context.Products.Where(p => p.CreatedAt == createdAt.ToUniversalTime());
> ```

> [!WARNING]
> **PostgreSQL rejects the local-time interfaces by default.** Npgsql maps `DateTime` to
> `timestamp with time zone`, which accepts only UTC values, so `ITimeStamps`, `ICreatedAt`,
> `IUpdatedAt` and `ISoftDelete` fail on write with:
>
> ```
> Cannot write DateTime with Kind=Local to PostgreSQL type 'timestamp with time zone',
> only UTC is supported.
> ```
>
> On PostgreSQL, prefer the `DateTimeOffset` interfaces (`ITimeStampsOffset`, `ISoftDeleteOffset`),
> or the UTC or Unix ones.
> To keep local time anyway, pin the columns to the untimezoned type:
>
> ```csharp
> modelBuilder.Entity<Product>().Property(p => p.CreatedAt).HasColumnType("timestamp without time zone");
> ```

> [!NOTE]
> **`DateTimeKind` is not persisted by any provider.** An entity read back from the database
> returns `CreatedAt.Kind == DateTimeKind.Unspecified`, even when it was written as UTC. Treat
> the value as UTC based on the interface the entity implements, not on its `Kind` — or use the
> `DateTimeOffset` interfaces, which do not have this problem.

> [!NOTE]
> **MySQL `DATETIME` defaults to whole seconds**, so two updates within the same second produce
> an identical `UpdatedAt`. Ask for sub-second precision where that matters:
>
> ```csharp
> [Precision(6)] public DateTime UpdatedAt { get; set; }
> ```

> [!WARNING]
> **MySQL stores `DateTimeOffset` to the whole second, even with `[Precision(6)]`.** Oracle's
> `MySql.EntityFrameworkCore` drops the fraction of a `DateTimeOffset` on its own; the
> `datetime(6)` column is not the problem, and `DateTime` keeps its microseconds. Where sub-second
> precision matters on MySQL, use the UTC interfaces, or store `DateTimeOffset` through a `DateTime`:
>
> ```csharp
> protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
> {
>     // Also covers DateTimeOffset? properties such as DeletedAt.
>     configurationBuilder.Properties<DateTimeOffset>().HaveConversion<UtcDateTimeConverter>();
> }
>
> public class UtcDateTimeConverter() : ValueConverter<DateTimeOffset, DateTime>(
>     v => v.UtcDateTime,
>     v => new DateTimeOffset(DateTime.SpecifyKind(v, DateTimeKind.Utc)));
> ```

## :wastebasket: Soft Delete

### Setup

1. Call `AddSoftDeleteFilter()` in your `DbContext`.

    ```csharp
    using Idam.EntityFrameworkCore.Timestamps.Extensions;

    public class MyDbContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddSoftDeleteFilter();

            base.OnModelCreating(modelBuilder);
        }
    }
    ```

2. Implement the appropriate soft delete interface (`ISoftDelete`, `ISoftDeleteUtc`, `ISoftDeleteOffset` or `ISoftDeleteUnix`).

    ```csharp
    using Idam.EntityFrameworkCore.Timestamps.Interfaces;

    /// Local DateTime
    public class Product : ISoftDelete
    {
        ...
        public DateTime? DeletedAt { get; set; }
    }
    
    /// UTC DateTime
    public class Product : ISoftDeleteUtc
    {
        ...
        public DateTime? DeletedAt { get; set; }
    }

    /// DateTimeOffset
    public class Product : ISoftDeleteOffset
    {
        ...
        public DateTimeOffset? DeletedAt { get; set; }
    }

    /// Unix Time
    public class Product : ISoftDeleteUnix
    {
        ...
        public long? DeletedAt { get; set; }
    }
    ```

### Operations

```csharp
// Restore a soft-deleted item
_context.Products.Restore(product);
await context.SaveChangesAsync();

// Permanently delete an item
_context.Products.ForceRemove(product);
await context.SaveChangesAsync();

// Check if item is deleted
bool isDeleted = product.Trashed();

// Query including soft-deleted items (using EF Core named query filters)
var productsWithDeleted = await _context.Products
    .IncludeTrashed()
    .ToListAsync();

// Query only soft-deleted items
var deletedProducts = await _context.Products
    .OnlyTrashed()
    .ToListAsync();
```

> [!NOTE]
> `IncludeTrashed()` leverages EF Core's named query filters feature to ignore only the default soft-delete global query filter, leaving other global query filters (like multi-tenancy) active.

> [!WARNING]
> **Cascade delete bypasses soft delete.** By the time the timestamp logic runs, EF Core has already marked the dependents of a removed parent as `Deleted`. The parent is soft-deleted, but any dependent that does **not** implement a soft-delete interface is **permanently deleted** — leaving a soft-deleted parent pointing at rows that no longer exist.
>
> For relationships where the parent is soft-deleted, either make the dependents soft-deletable too, or switch the relationship to `DeleteBehavior.Restrict`:
>
> ```csharp
> modelBuilder.Entity<Order>()
>     .HasMany(o => o.Lines)
>     .WithOne()
>     .OnDelete(DeleteBehavior.Restrict);
> ```

> [!NOTE]
> `ForceRemove()` stamps `DeletedAt` on the entity before removing it, which is how it tells the soft-delete logic to let the delete through. If `SaveChanges()` is never called or throws, the entity stays tracked as `Deleted`, so the next `SaveChanges()` on that context still deletes it permanently. The instance itself keeps the stamped `DeletedAt`, though: attach it elsewhere (for example `Update()` on a new context) and saving stores it as soft-deleted. Discard or reload the instance after a failed force-remove.

## :art: Customization

### Custom Field Names

```csharp
public class Product : ITimeStamps, ISoftDelete
{
    [Column("AddedAt")]
    public DateTime CreatedAt { get; set; }
    
    [Column("ModifiedAt")]
    public DateTime UpdatedAt { get; set; }
    
    [Column("RemovedAt")]
    public DateTime? DeletedAt { get; set; }
}
```

### Individual Interfaces

```csharp
public class Product : ICreatedAt { }
public class Product : ICreatedAtUtc { }
public class Product : ICreatedAtOffset { }
public class Product : ICreatedAtUnix { }

public class Product : IUpdatedAt { }
public class Product : IUpdatedAtUtc { }
public class Product : IUpdatedAtOffset { }
public class Product : IUpdatedAtUnix { }

public class Product : ISoftDelete { }
public class Product : ISoftDeleteUtc { }
public class Product : ISoftDeleteOffset { }
public class Product : ISoftDeleteUnix { }
```

### Injecting the Clock

Timestamps can come from a `TimeProvider` instead of the system clock. This is most useful with a
fake one in tests, so timestamps are known up front instead of waiting for the clock to move.

When the context is registered with `AddDbContext`, registering a `TimeProvider` is enough. The
`AddTimeStampsInterceptor()` you already have picks it up:

```csharp
builder.Services.AddSingleton<TimeProvider>(fakeTimeProvider);
```

Otherwise, pass one explicitly:

```csharp
optionsBuilder.AddTimeStampsInterceptor(timeProvider);
```

Every write path reads the same clock: the interceptor, `ChangeTracker.AddTimestamps()` and
`ForceRemove()`. They look for it in this order:

1. A `TimeProvider` passed to `AddTimeStampsInterceptor(timeProvider)`.
2. The application's `TimeProvider` service.
3. The system clock.

Because they all agree, registering the interceptor twice, or keeping `SaveChanges` overrides next to
it, cannot mix clocks.

`FakeTimeProvider` from the
[`Microsoft.Extensions.TimeProvider.Testing`](https://www.nuget.org/packages/Microsoft.Extensions.TimeProvider.Testing)
package works well here. The local-time interfaces convert with `TimeProvider.LocalTimeZone`. When
that is the machine's zone, as with the system clock, values have `Kind=Local`. Any other zone
produces `Kind=Unspecified`, because `Local` means the machine's zone. `FakeTimeProvider` defaults to
UTC, which keeps local values identical on every machine; call `SetLocalTimeZone(...)` to pick another
zone.

## :arrows_counterclockwise: Migration Guide

### Moving to the interceptor

Replace the two `SaveChanges` overrides with a single registration:

```diff
-public override int SaveChanges(bool acceptAllChangesOnSuccess)
-{
-    ChangeTracker.AddTimestamps();
-    return base.SaveChanges(acceptAllChangesOnSuccess);
-}
-
-public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
-{
-    ChangeTracker.AddTimestamps();
-    return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
-}
+protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
+{
+    optionsBuilder.AddTimeStampsInterceptor();
+}
```

Remove the overrides when you add the interceptor. Keeping both is harmless, since both read the
same clock, but it computes every timestamp twice and also refreshes `UpdatedAt` when soft-deleting.

`AddTimestamps()` remains public and supported; nothing breaks if you keep the overrides.

### A registered `TimeProvider` is now used

> [!IMPORTANT]
> **Behaviour change.** `AddTimeStampsInterceptor()`, `ChangeTracker.AddTimestamps()` and
> `ForceRemove()` used to read the system clock unconditionally. They now read a `TimeProvider`
> registered in dependency injection, when the context comes from `AddDbContext`.

Nothing changes if you register no `TimeProvider`, or register `TimeProvider.System`. If your
application or its integration tests register a different one, timestamps now follow it. Tests that
compared `CreatedAt` with `DateTime.Now` need to compare it with that provider's time instead.

To keep the system clock regardless of what is registered, pass it explicitly. An explicit clock
always wins:

```csharp
optionsBuilder.AddTimeStampsInterceptor(TimeProvider.System);
```

## :handshake: How to Contribute

We welcome contributions from the community! Here's how you can help make this project better:

### Getting Started

1. **Fork the Repository**: Click the "Fork" button at the top right of this repository.
2. **Clone Your Fork**: 
   ```shell
   git clone https://github.com/your-username/Idam.EntityFrameworkCore.Timestamps.git
   cd Idam.EntityFrameworkCore.Timestamps
   ```
3. **Create a Branch**: Create a feature branch for your changes:
   ```shell
   git checkout -b feature/your-feature-name
   ```

### Making Changes

1. **Make Your Changes**: Implement your feature or bug fix.
2. **Write Tests**: Add or update tests to cover your changes.
3. **Follow Code Style**: Ensure your code follows the project's existing code style and conventions.
4. **Commit Your Changes**: Write clear, concise commit messages:
   ```shell
   git commit -m "Add: brief description of your changes"
   ```

### Submitting Your Contribution

1. **Push to Your Fork**:
   ```shell
   git push origin feature/your-feature-name
   ```
2. **Open a Pull Request**: 
   - Go to the original repository and click "New Pull Request"
   - Select your branch and provide a clear description of your changes
   - Reference any related issues using `#issue-number`

### Guidelines

- **Code Quality**: Ensure your code is clean and well-documented
- **Testing**: All tests must pass before submission
- **Documentation**: Update README.md or other docs if your changes introduce new features
- **Issues**: Check existing issues before starting work to avoid duplicates
- **Communication**: Be respectful and constructive in discussions

### Reporting Issues

Found a bug or have a feature request? Please [open an issue](https://github.com/idamachmadfaizin/Idam.EntityFrameworkCore.Timestamps/issues) with:
- A clear title and description
- Steps to reproduce (for bugs)
- Expected vs. actual behavior
- Your environment details (.NET version, EF Core version, etc.)

### Questions?

If you have any questions, feel free to open a discussion or reach out to the maintainers.

Thank you for your contributions! :tada:
