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
# Run all tests, every provider (CI mirrors this). Needs a running Docker daemon.
dotnet test Idam.EntityFrameworkCore.Timestamps.Tests

# Fast loop, no container needed
dotnet test Idam.EntityFrameworkCore.Timestamps.Tests --filter "Provider=Sqlite"

# One provider at a time
dotnet test Idam.EntityFrameworkCore.Timestamps.Tests --filter "Provider=PostgreSql"

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

- Framework: xUnit with `[Fact]`
- Test data: `Bogus`, via `BaseTest.Fake<T>()` / `FakeMany<T>(n)` (wrapping `BaseEntityFaker<T>`)
- Global usings in `Usings.cs` — only `global using Xunit`

### Provider matrix

Every test runs against **SQLite, SQL Server, MySQL and PostgreSQL** — real relational providers
only. SQL Server, MySQL and PostgreSQL run in containers via Testcontainers, so `dotnet test`
needs a Docker daemon; SQLite runs in-process and needs nothing.

The EF Core InMemory provider is deliberately **not** used. It is not relational: it evaluates
LINQ client-side and has no column types, so it silently passes queries that no database can
translate and timestamps that no column can store. It hid real bugs in this repo behind a green
build. Do not add it back as a shortcut when a container feels slow — use
`--filter "Provider=Sqlite"` instead.

- `Tests/Fixtures/` holds one fixture per provider, each behind a `[CollectionDefinition]` so the
  container starts once per run. Every test still gets its own database.
- `BaseTest<TFixture>` takes the fixture and exposes `Context`, `AddAsync`, `AddRangeAsync`,
  `DeleteAsync`, and `ReloadAsync<T>(id)` for asserting what the provider actually stored.
- Each suite is an `abstract class XTests<TFixture> : BaseTest<TFixture>` plus one sealed subclass
  per provider, tagged `[Trait("Provider", "...")]` and `[Collection(...)]`. Add a test to the
  abstract class and it runs everywhere.

### Provider quirks the tests work around

- **PostgreSQL**: Npgsql maps `DateTime` to `timestamp with time zone` and rejects `Kind=Local`,
  so the local-time interfaces need `timestamp without time zone`. See
  `TestDbContext.MapLocalDateTimesForNpgsql`.
- **MySQL**: `DATETIME` defaults to whole seconds, which would make two updates in the same second
  indistinguishable. Test entities use `[Precision(6)]`. The container runs as `root` because the
  module's default user may not `CREATE DATABASE`.
- **MySQL provider**: Pomelo has no EF Core 10 release (latest is 9.0.0), so this repo uses
  Oracle's `MySql.EntityFrameworkCore`.
- **All providers**: `DateTimeKind` is never persisted; a UTC value returns as `Unspecified`.

## Code Style

- File-scoped namespaces
- Nullable enabled, ImplicitUsings enabled
- C# 14 extension blocks (not traditional extension methods)

## Key Architecture

- Timestamp logic lives in `DbContextExtensions.UpdateTimeStamps` — switches on interface type to set CreatedAt/UpdatedAt
- Soft delete logic in `DbContextExtensions.UpdateSoftDelete` — converts `Deleted` state to `Modified` with `DeletedAt` set
- Soft delete query filter registered via `ModelBuilder.AddSoftDeleteFilter()`, named `"Idam.EntityFrameworkCore.Timestamps.SoftDelete"` (in `SoftDeleteFilters.Default`)
- `IncludeTrashed()` uses `IgnoreQueryFilters` with the named filter to selectively bypass soft-delete while leaving other filters active
- Interfaces hierarchy: `ITimeStampBase` → `ICreatedAt`/`IUpdatedAt`/`ITimeStamps`, `ISoftDeleteBase` → `ISoftDelete`/`ISoftDeleteUtc`/`ISoftDeleteUnix`

## Commit Convention

Format: Conventional Commits.

`CHANGELOG.md` is generated from commit subjects by [git-cliff](https://git-cliff.org)
(`cliff.toml`), so never edit it by hand — fix `cliff.toml` or the commit instead.

- A `feat`/`fix` subject is a changelog line: write it for package consumers.
- Breaking change: `feat!:` / `fix!:`, plus a `BREAKING CHANGE:` footer saying what to migrate.
- Left out: scopes `sample` and `test(s)`, and types `chore`, `docs`, `test`, `ci`, `build`, `style`.
- Pre-release tags (`-alpha`, `-beta`, `-rc`) fold into the next stable version.

## Release

```bash
npx git-cliff@2 --tag vX.Y.Z -o CHANGELOG.md
git commit -am "chore(release): vX.Y.Z"
git tag vX.Y.Z && git push && git push origin vX.Y.Z
```

The tag triggers `publish_to_nuget.yml`: tests, NuGet push with the version taken from the tag,
then a GitHub release whose notes are that version's changelog section. A pre-release tag
(`vX.Y.Z-alpha1`) needs no changelog commit.
