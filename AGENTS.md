# AGENTS.md

## Project

NuGet library for EF Core entity timestamps (CreatedAt, UpdatedAt, DeletedAt) and soft delete. Supports three timestamp formats: Local DateTime, UTC DateTime, and Unix milliseconds.

## SDK & TFM

Requires .NET 10.0 SDK (`global.json` pins `10.0.0`, `rollForward: latestMajor`). All projects target `net10.0`. Language version is `latestMajor` — the codebase uses C# 14 extension blocks (`extension(Type t) { }` syntax).

## Solution Structure

```
Idam.EntityFrameworkCore.Timestamps/   ← library (NuGet package, GeneratePackageOnBuild)
Idam.EntityFrameworkCore.Timestamps.Tests/  ← xUnit tests
Idam.EntityFrameworkCore.Timestamps.Sample/ ← ASP.NET Core sample app (SQLite)
```

Uses `.slnx` solution format (not traditional `.sln`).

## Commands

```bash
# Run all tests (CI mirrors this)
dotnet test Idam.EntityFrameworkCore.Timestamps.Tests

# Build with tests
dotnet build Idam.EntityFrameworkCore.Timestamps.Tests --configuration Release
dotnet test Idam.EntityFrameworkCore.Timestamps.Tests --configuration Release

# Pack the library (also happens automatically on build)
dotnet build Idam.EntityFrameworkCore.Timestamps --configuration Release
```

No lint, formatter, or typecheck tooling beyond the compiler. CI only builds/tests the test project.

## Package Management

Central Package Management enabled. All NuGet versions are in `Directory.Packages.props`. Never add `Version` attributes to individual `<PackageReference>` elements in `.csproj` files.

## Testing

- Framework: xUnit with `[Fact]` (no `[Theory]` usage observed)
- Test data: `Bogus` library via `BaseEntityFaker<T>`
- Database: EF Core InMemory provider, one unique DB per test (`Guid.NewGuid()` in name)
- Test base class: `BaseTest` provides `AddAsync`, `AddRangeAsync`, `DeleteAsync` helpers; each test creates and destroys its own `TestDbContext`
- Global usings in `Usings.cs` — only `global using Xunit`

## Code Style

- File-scoped namespaces
- Nullable enabled, ImplicitUsings enabled
- C# 14 extension blocks (not traditional extension methods)
- No `.editorconfig` or analyzer config

## Key Architecture

- Timestamp logic lives in `DbContextExtensions.UpdateTimeStamps` — switches on interface type to set CreatedAt/UpdatedAt
- Soft delete logic in `DbContextExtensions.UpdateSoftDelete` — converts `Deleted` state to `Modified` with `DeletedAt` set
- Soft delete query filter registered via `ModelBuilder.AddSoftDeleteFilter()`, named `"Idam.EntityFrameworkCore.Timestamps.SoftDelete"` (in `SoftDeleteFilters.Default`)
- `IncludeTrashed()` uses `IgnoreQueryFilters` with the named filter to selectively bypass soft-delete while leaving other filters active
- Interfaces hierarchy: `ITimeStampBase` → `ICreatedAt`/`IUpdatedAt`/`ITimeStamps`, `ISoftDeleteBase` → `ISoftDelete`/`ISoftDeleteUtc`/`ISoftDeleteUnix`

## Commit Convention

Format: Conventional Commits.
