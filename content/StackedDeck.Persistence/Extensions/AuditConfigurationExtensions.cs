using System;
using System.Linq;
using System.Text.Json;

using Audit.Core;
using Audit.Core.ConfigurationApi;
using Audit.EntityFramework;

using Microsoft.EntityFrameworkCore;

using StackedDeck.Persistence.Template.Entities;

namespace StackedDeck.Persistence.Template.Extensions;

public static class AuditConfiguratorExtensions
{
    /// <summary>
    /// Uses a custom auditing configuration from the 'StackedDeck.Persistence' project template,
    /// which uses a single <see cref="AuditLog"/> entity to store the audit trails.
    /// </summary>
    /// <param name="configurator">The Audit.NET configurator.</param>
    /// <remarks>
    /// Supports soft-deletion & regular deletion events.
    /// </remarks>
    public static ICreationPolicyConfigurator UseStackedDeckAuditing(this IConfigurator configurator)
    {
        return configurator
             .UseEntityFramework(config =>
             {
                 config
                     .AuditTypeMapper(eventType => typeof(IAuditableEntity).IsAssignableFrom(eventType)
                         ? typeof(AuditLog)
                         : null)
                     .AuditEntityAction<AuditLog>((_, eventEntry, auditLog) =>
                     {
                         var pk = eventEntry.PrimaryKey.Values.FirstOrDefault();
                         auditLog.EntityId = pk?.ToString();
                         auditLog.EntityName = eventEntry.EntityType.Name;
                         auditLog.Action = DetermineAuditLogAction(eventEntry);
                         auditLog.Value = JsonSerializer.Serialize(eventEntry.ColumnValues);

                         if (eventEntry.Action == nameof(AuditAction.Update))
                         {
                             auditLog.Delta = JsonSerializer.Serialize(eventEntry.Changes);
                         }

                         auditLog.Timestamp = DateTimeOffset.UtcNow;
                     })
                     .IgnoreMatchedProperties(true);
             });
    }

    /// <summary>
    /// Determines the Audit log action based on the event entry metadata.
    /// </summary>
    /// <param name="eventEntry">The Audit.NET event entry.</param>
    private static string DetermineAuditLogAction(EventEntry eventEntry)
    {
        if (eventEntry.Action != nameof(AuditAction.Update) || eventEntry.Changes is not { } changes)
        {
            return eventEntry.Action;
        }

        var isDeletedColumnName = eventEntry
            .GetEntry()
            ?.Metadata
            .FindProperty(nameof(ISoftDeletableEntity.IsDeleted))
            ?.GetColumnName();

        return !string.IsNullOrWhiteSpace(isDeletedColumnName) &&
            changes.Any(c => c.ColumnName.Equals(isDeletedColumnName, StringComparison.OrdinalIgnoreCase)
                    && c.NewValue is true)
            ? nameof(AuditAction.SoftDelete)
            : eventEntry.Action;
    }
}
