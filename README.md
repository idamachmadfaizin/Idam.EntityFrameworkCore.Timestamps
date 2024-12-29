# Idam.EFTimestamps

[![NuGet](https://img.shields.io/nuget/v/Idam.EFTimestamps.svg)](https://www.nuget.org/packages/Idam.EFTimestamps) [![.NET](https://github.com/ronnygunawan/RG.RazorMail/actions/workflows/CI.yml/badge.svg)](https://github.com/idamachmadfaizin/Idam.EFTimestamps/actions/workflows/test.yml)

A lightweight .NET library that simplifies timestamp management in Entity Framework Core by automatically handling creation, update, and deletion timestamps. The library supports multiple timestamp formats including DateTime, UTC DateTime, and Unix Time Milliseconds.

## :star: Support

If you find this library helpful, please consider giving it a star! Your support helps make it better.

## :rocket: Features

- Automatic management of entity timestamps (Created, Updated, Deleted).
- Multiple timestamp format support:
  - DateTime.
  - UTC DateTime.
  - Unix Time Milliseconds ([learn more](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset.tounixtimemilliseconds)).
- Built-in soft delete functionality.
- Seamless integration with existing EF Core projects.
- Customizable timestamp field names.
- Individual timestamp interfaces for flexibility.

## :package: Installation

Using Package Manager Console:
```shell
Install-Package Idam.EFTimestamps
```

Using .NET CLI
```shell
dotnet add package Idam.EFTimestamps
```

## :wrench: Basic Setup

### 1. Configure DbContext

Add `AddTimestamps()` in your DbContext.

```csharp
...
using Idam.EFTimestamps.Extensions;

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

### 2. Define your entity 

Choose the appropriate interface based on your timestamp format needs (`ITimeStamps` or `ITimeStampsUtc` or `ITimeStampsUnix`).

```csharp
using Idam.EFTimestamps.Interfaces;

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

## Soft Delete Feature

### Setup

1. Update your DbContext 

   Add `AddSoftDeleteFilter()` in your context.

    ```csharp
    using Idam.EFTimestamps.Extensions;

    public class MyDbContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddSoftDeleteFilter();

            base.OnModelCreating(modelBuilder);
        }
    }
    ```

2. Implement soft delete in your entities

   Choose the appropriate interface based on your timestamp format needs (`ISoftDelete` or `ISoftDeleteUtc` or `ISoftDeleteUnix`).

    ```csharp
    using Idam.EFTimestamps.Interfaces;

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

### Soft Delete Operations

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

### Individual Timestamp Interfaces

```csharp
public class Product : ICreatedAt { }      // Only creation time
public class Product : ICreatedAtUtc { }   // Only creation time (UTC)
public class Product : ICreatedAtUnix { }  // Only creation time (Unix time)

public class Product : IUpdatedAt { }      // Only update time
public class Product : IUpdatedAtUtc { }   // Only update time (UTC)
public class Product : IUpdatedAtUnix { }  // Only update time (Unix time)

public class Product : ISoftDelete { }     // Only soft delete
public class Product : ISoftDeleteUtc { }  // Only soft delete (UTC)
public class Product : ISoftDeleteUnix { } // Only soft delete (Unix time)
```

## :arrows_counterclockwise: Migration Guide

When upgrading from version 8.0.0:
- Replace ITimeStamps with ITimeStampsUtc. 
- Replace ISoftDelete with ISoftDeleteUtc.
