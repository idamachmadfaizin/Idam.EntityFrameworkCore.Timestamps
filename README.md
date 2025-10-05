# Idam.EntityFrameworkCore.Timestamps

[![NuGet](https://img.shields.io/nuget/v/Idam.EntityFrameworkCore.Timestamps.svg)](https://www.nuget.org/packages/Idam.EntityFrameworkCore.Timestamps)
[![Build Status](https://github.com/idamachmadfaizin/Idam.EntityFrameworkCore.Timestamps/actions/workflows/test.yml/badge.svg)](https://github.com/idamachmadfaizin/Idam.EntityFrameworkCore.Timestamps/actions/workflows/test.yml)

A .NET library for entity timestamps and softdelete. Easily manage CreatedAt, UpdatedAt, and DeletedAt fields with support for DateTime, UTC DateTime, and Unix time (milliseconds).

## :star: Support

If you find this library helpful, please consider giving it a star! Your support helps make it better.

## :rocket: Features

- Automatic handling of entity timestamps (**CreatedAt**, **UpdatedAt**, **DeletedAt**).
- Built-in **soft delete** functionality with global query filters.
- Support for multiple timestamp formats:
  - Local `DateTime`
  - `UTC DateTime`.
  - `Unix Time (milliseconds)` ([learn more](https://currentmillis.com)) ([docs](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset.tounixtimemilliseconds)).
- Seamless integration with existing **EF Core**.
- Customizable field names with `[Column]` attribute.
- Flexible interfaces for individual timestamp requirements.

## :package: Installation

.NET CLI
```shell
dotnet add package Idam.EntityFrameworkCore.Timestamps
```

## :wrench: Basic Setup

### 1. Configure `DbContext`

Call `AddTimestamps()` in your `DbContext` before saving changes.

```csharp
...
using Idam.EntityFrameworkCore.Timestamps.Extensions;

public class MyDbContext : DbContext
{
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
}
```

### 2. Define Your Entity 

Implement the appropriate timestamps interface (`ITimeStamps` or `ITimeStampsUtc` or `ITimeStampsUnix`).

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

/// Unix Time
public class Product : ITimeStampsUnix
{
    ...
    public long CreatedAt { get; set; }
    public long UpdatedAt { get; set; }
}
```

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

2. Implement the appropriate soft delete interface (`ISoftDelete` or `ISoftDeleteUtc` or `ISoftDeleteUnix`).

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

// Query including soft-deleted items
var deletedProducts = await _context.Products
    .IgnoreQueryFilters()
    .Where(x => x.DeletedAt != null)
    .ToListAsync();
```

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
public class Product : ICreatedAtUnix { }

public class Product : IUpdatedAt { }
public class Product : IUpdatedAtUtc { }
public class Product : IUpdatedAtUnix { }

public class Product : ISoftDelete { }
public class Product : ISoftDeleteUtc { }
public class Product : ISoftDeleteUnix { }
```

## :arrows_counterclockwise: Migration Guide

