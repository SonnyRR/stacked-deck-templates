using System;
using System.Linq;
using System.Text.Json;

#if (UseAuditNet)
using Audit.Core;
using Audit.EntityFramework;

#endif
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using StackedDeck.Persistence.Template.Entities;

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

        services.AddDbContext<ApplicationDbContext>(opt =>
#if (UseMssqlProvider)
            opt.UseSqlServer(connectionString)
#elif (UsePostgresProvider)
            opt.UseNpgsql(connectionString).UseSnakeCaseNamingConvention()
#elif (UseSqliteProvider)
            opt.UseSqlite(connectionString)
#else
            opt.UseSqlite(connectionString)
#endif
#if (UseAuditNet)
                .AddInterceptors(new AuditSaveChangesInterceptor())
#endif
        );

#if (UseAuditNet)
        // Single AuditLog table configuration for all entity types.
        // For per-entity configuration options, see: https://github.com/thepirat000/Audit.NET/tree/master/src/Audit.EntityFramework
        Audit.Core.Configuration.Setup()
            .UseEntityFramework(config =>
            {
                config
                    .AuditTypeMapper(_ => typeof(AuditLog))
                    .AuditEntityAction<AuditLog>((_, eventEntry, auditLog) =>
                    {
                        var pk = eventEntry.PrimaryKey.Values.FirstOrDefault();
                        auditLog.EntityId = pk?.ToString();
                        auditLog.EntityName = eventEntry.EntityType.Name;
                        auditLog.Action = eventEntry.Action;
                        auditLog.Value = JsonSerializer.Serialize(eventEntry.ColumnValues);

                        if (eventEntry.Action == "Update")
                        {
                            auditLog.Delta = JsonSerializer.Serialize(eventEntry.Changes);
                        }

                        auditLog.Timestamp = DateTimeOffset.UtcNow;
                    })
                    .IgnoreMatchedProperties(true);
            });
#endif

        return services;
    }
}
