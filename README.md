# Idam.EFTimestamps

[![NuGet](https://img.shields.io/nuget/v/Idam.EFTimestamps.svg)](https://www.nuget.org/packages/Idam.EFTimestamps) [![.NET](https://github.com/ronnygunawan/RG.RazorMail/actions/workflows/CI.yml/badge.svg)](https://github.com/idamachmadfaizin/Idam.EFTimestamps/actions/workflows/test.yml)

A library for handling Timestamps and SoftDelete as UTC DateTime or Unix in EntityFramework.

## Give a Star! :star:

If you like or are using this project please give it a star. Thanks!

## Features

- Soft delete (DeletedAt).
- Timestamps (CreatedAt, UpdatedAt).

> Both features support UTC DateTime
> and [Unix Time Milliseconds](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset.tounixtimemilliseconds)
> format.
>
>Example of Unix Time Milliseconds: [currentmillis.com](https://currentmillis.com)

## Get started

Run this command to install

```sh
Install-Package Idam.EFTimestamps
```

or

```sh
dotnet add package Idam.EFTimestamps
```

## Usage

### Using Timestamps

1. Add `AddTimestamps()` in your context.

    ```cs
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

2. Implement an Interface (`ITimeStamps` or `ITimeStampsUtc` or `ITimeStampsUnix`) to your entity.

    ```cs
    using Idam.EFTimestamps.Interfaces;

    /// BaseEntity
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
    }
    
    /// Using local DateTime Format
    public class Dt : BaseEntity, ITimeStamps
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// Using UTC DateTime Format
    public class DtUtc : BaseEntity, ITimeStampsUtc
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// Using Unix Format
    public class Unix : ITimeStampsUnix
    {
        public long CreatedAt { get; set; }
        public long UpdatedAt { get; set; }
    }
    ```

### Using SoftDelete

1. Add `AddTimestamps()` and `AddSoftDeleteFilter()` in your context.

    ```cs
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddSoftDeleteFilter();

            base.OnModelCreating(modelBuilder);
        }
    }
    ```

2. Implement an Interface (`ISoftDelete` or `ISoftDeleteUtc` or `ISoftDeleteUnix`) to your entity.

    ```cs
    using Idam.EFTimestamps.Interfaces;

    /// Using local DateTime Format
    public class Dt : BaseEntity, ISoftDelete
    {
        public DateTime? DeletedAt { get; set; }
    }
    
    /// Using UTC DateTime Format
    public class Dt : BaseEntity, ISoftDeleteUtc
    {
        public DateTime? DeletedAt { get; set; }
    }

    /// Using Unix Format
    public class Unix : BaseEntity, ISoftDeleteUnix
    {
        public long? DeletedAt { get; set; }
    }
    ```

#### Restore

The SoftDelete has a `Restore()` function, so you can restore the deleted data.

```cs
using Idam.EFTimestamps.Extensions;

/// Your context
public class MyDbContext : DbContext
{
    public DbSet<Dt> Dts { get; set; }
}

/// Dt Controller
public class DtController
{
    readonly MyDbContext _context;

    public async Task<IActionResult> RestoreAsync(Dt dt)
    {
        var restored = _context.Dts.Restore(dt);
        await context.SaveChangesAsync();
        
        return Ok(restored);
    }
}
```

#### Force Remove

The SoftDelete has a `ForceRemove()` function, so you can permanently remove the data.

```cs
/// Dt Controller
public class DtController
{
    readonly MyDbContext _context;

    public async Task<IActionResult> ForceRemoveAsync(Dt dt)
    {
        _context.Dts.ForceRemove(dt);
        await context.SaveChangesAsync();
        
        return Ok();
    }
}
```

#### Trashed

The SoftDelete has a `Trashed()` function to check if current data is deleted.

```cs
/// Dt Controller
public class DtController
{
    public IActionResult IsDeleted(Dt dt)
    {
        bool isDeleted = dt.Trashed();
        
        return Ok(isDeleted);
    }
}
```

> The `Trashed()` function only shows when your entity implements an interface `ISoftDelete` or `ISoftDeleteUtc` or `ISoftDeleteUnix`.

#### Ignore global soft delete filter

By default the deleted data filtered from the query, if you want to get the deleted data you can ignore the global
soft delete filter by using `IgnoreQueryFilters()`.

```cs
/// Dt Controller
public class DtController
{
    readonly MyDbContext _context;

    public async Task<IActionResult> GetAllDeletedAsync()
    {
        var deleteds = await _context.Dts
            .IgnoreQueryFilters()
            .Where(x => x.DeletedAt != null)
            .ToListAsync();

        return Ok(deleteds);
    }
}
```

### Using Custom TimeStamps fields

By default, the TimeStamps interface uses CreatedAt, UpdatedAt, and DeletedAt as field names. To customize the
TimeStamps fields simplify just add ColumnAttribute to the fields.

## Migrating

Migrating from 8.0.0

1. Rename the ITimeStamps to ITimeStampsUtc.
2. Rename the ISoftDelete to ISoftDeleteUtc.
