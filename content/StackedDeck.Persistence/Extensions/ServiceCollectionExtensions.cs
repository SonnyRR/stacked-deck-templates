using System;

#if (UseAuditNet)
using Audit.Core;
using System.Linq;
using System.Text.Json;

using StackedDeck.Persistence.Template.Entities;

#endif
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace StackedDeck.Persistence.Template.Extensions;

/// <summary>
/// Extension methods for configuring persistence services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds persistence services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="connectionString">The database connection string.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, string connectionString)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

#if (UseMssqlProvider)
        services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlServer(connectionString));
#elif (UsePostgresProvider)
        services.AddDbContext<ApplicationDbContext>(opt => opt
                .UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention());
#elif (UseSqliteProvider)
        services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlite(connectionString));
#endif

#if (UseAuditNet)
        Audit.Core.Configuration.Setup()
            .UseEntityFramework(config =>
            {
#if (UseSeparateAuditTables)
                config
                    .AuditTypeNameMapper(typeName => "Audit_" + typeName)
                    .AuditEntityAction((ev, entry, auditEntity) =>
                    {
                        ((dynamic)auditEntity).AuditDate = DateTime.UtcNow;
                    })
                    .IgnoreMatchedProperties(true);
#else
                config
                    .AuditTypeMapper(_ => typeof(AuditLog))
                    .AuditEntityAction<AuditLog>((_, eventEntry, auditLog) =>
                    {
                        var pk = eventEntry.PrimaryKey.Values.FirstOrDefault();
                        auditLog.EntityId = pk?.ToString();
                        auditLog.EntityName = eventEntry.EntityType.Name;
                        auditLog.Action = eventEntry.Action;
                        auditLog.Changes = JsonSerializer.Serialize(eventEntry.ColumnValues);
                        auditLog.Timestamp = DateTimeOffset.UtcNow;
                    })
                    .IgnoreMatchedProperties(true);
#endif
            });
#endif

        return services;
    }
}
