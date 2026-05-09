# Stacked Deck Persistence Project Template

A production-ready `Entity Framework Core` Persistence layer project template,
that comes with batteries included and sensible defaults.

TFM: `net10.0`
Identifier: `sd-persistence`

View all configuration parameters:

```sh
dotnet new sd-persistence -h
```

## 🧩 Features

> [!NOTE]
> Below are mentioned some of the key features of this template. I strongly
> encourage reviewing the source code documents for getting an insight to
> the full suite of features. The project contains in-place implementations
> for automatic audit metadata injection, soft deletion support,
> multiple provider support and more.

### 🗄️ Database Providers

This template supports multiple database providers out-of-the-box. You can choose
between:

- **MSSQL** - Microsoft SQL Server or Azure SQL Database
- **PostgreSQL** - PostgreSQL with `EFCore.NamingConventions` for snake_case naming
- **SQLite** - SQLite for local development or lightweight deployments

To select a provider, use the `--provider` flag:

```sh
dotnet new sd-persistence -n MyProject --provider PostgreSQL
```

> [!TIP]
> The default provider is `MSSQL`. For local development with minimal setup,
> consider using `SQLite`.

### 📊 Auditing

This template provides comprehensive audit trail support through multiple approaches:

#### 🔍 Temporal Tables (MSSQL Only)

When using `MSSQL` as your provider, you can enable temporal tables for built-in
history tracking. Entity configurations must inherit from
`TemporalEntityTypeConfiguration<T>` and override `Configure()` to invoke
`base.Configure()`:

```csharp
public class TodoEntityTypeConfiguration : TemporalEntityTypeConfiguration<Todo>
{
    public override void Configure(EntityTypeBuilder<Todo> builder)
    {
        base.Configure(builder);

        builder.HasKey(e => e.Id);
    }
}
```

```sh
dotnet new sd-persistence -n MyProject --provider MSSQL --auditing Temporal
```

> [!WARNING]
> Temporal tables are only supported with `MSSQL`. Using `--auditing Temporal`
> with `--provider PostgreSQL` or `--provider SQLite` will emit a warning during
> project creation.

#### 🕵️ Audit.NET

For cross-provider support, this template integrates with `Audit.EntityFramework.Core`
to provide comprehensive audit logging. This approach works with all supported providers
and offers advanced features like custom audit stores and event enrichment.

```sh
dotnet new sd-persistence -n MyProject --auditing AuditNet
```

#### 🚫 None

No audit trail support. The `IAuditableEntity` interface and
`AuditInterceptor` are still included to assign and maintain the
`CreatedAt`, `CreatedBy`, `UpdatedAt` and `UpdatedBy` metadata on
entities directly, but no change histories or snapshots are stored.

```sh
dotnet new sd-persistence -n MyProject --auditing None
```

> [!NOTE]
> The default auditing approach is `AuditNet`.

### 🏷️ Entity Interfaces

This template ships with two optional entity interfaces that can be implemented
by domain entities to provide standardized behavior.

#### ✍️ IAuditableEntity

Defines properties for entities that track creation and modification metadata:

- `CreatedBy` - The user who created the entity
- `CreatedAt` - The timestamp when the entity was created
- `UpdatedBy` - The user who last modified the entity
- `UpdatedAt` - The timestamp when the entity was last modified

This interface is implemented automatically by the `AuditInterceptor`, which
sets the appropriate metadata values before `SaveChanges()` is invoked.

#### 🧹 ISoftDeletableEntity

Defines properties for entities that support soft deletion:

- `IsDeleted` - A flag indicating whether the entity has been soft-deleted
- `DeletedBy` - The user who deleted the entity
- `DeletedAt` - The timestamp when the entity was deleted

This interface is implemented automatically by the `SoftDeleteInterceptor`,
which transforms `Delete` operations into `Update` operations, setting the
appropriate metadata instead of removing the record from the database.

### 🔌 EF Core Interceptors

This template provides two interceptors that extend the default EF Core behavior
for automatic metadata management.

#### 🎯 AuditInterceptor

A custom interceptor that extends `AuditSaveChangesInterceptor` when using
`Audit.NET`, or `SaveChangesInterceptor` otherwise. This interceptor automatically
sets the `CreatedAt`, `CreatedBy`, `UpdatedAt` and `UpdatedBy` properties on
entities that implement `IAuditableEntity` before `SaveChanges()` is invoked.

#### 🧹 SoftDeleteInterceptor

A custom interceptor that transforms delete operations into update operations
for entities that implement `ISoftDeletableEntity`. Instead of removing the record
from the database, it sets the `IsDeleted`, `DeletedAt` and `DeletedBy` properties.

> [!TIP]
> Both interceptors are registered in the `IServiceCollection` via the provided
> `ServiceCollectionExtensions` class. Review the extensions for customization options.

### ⚙️ Dependency Injection

This template leverages `Microsoft.Extensions.DependencyInjection.Abstractions`
for loose coupling and testability. All services are registered in the DI container
following convention-based registration patterns. The `ServiceCollectionExtensions`
class provides methods for registering the `ApplicationDbContext` and all interceptors.

### 🏗️ Fluent API Configuration

Entity configurations are applied automatically via `ApplyConfigurationsFromAssembly`,
following the standard EF Core Fluent API pattern. This allows for centralized configuration
management without manual registration of each entity type configuration.