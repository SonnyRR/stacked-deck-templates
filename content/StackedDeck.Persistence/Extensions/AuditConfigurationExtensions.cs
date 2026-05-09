using System;
using System.Linq;
using System.Text.Json;

using Audit.Core;
using Audit.Core.ConfigurationApi;
using Audit.EntityFramework;

using Microsoft.EntityFrameworkCore;

using StackedDeck.Persistence.Template.Entities;
using StackedDeck.Persistence.Template.Enums;

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
    /// <returns>
    /// A pre-configured instance of <see cref="ICreationPolicyConfigurator"/> with sensible defaults
    /// from the 'StackedDeck.Persistence' project template.
    /// </returns>
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
                         var primaryKey = eventEntry.PrimaryKey.Values.FirstOrDefault();
                         auditLog.EntityId = primaryKey?.ToString();
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
    /// <returns>
    /// An <see cref="AuditAction"/> value.
    /// </returns>
    private static AuditAction DetermineAuditLogAction(EventEntry eventEntry)
    {
        if (eventEntry.Action != nameof(AuditAction.Update) || eventEntry.Changes is not { } changes)
        {
            return Enum.Parse<AuditAction>(eventEntry.Action);
        }

        var isDeletedColumnName = eventEntry
            .GetEntry()
            ?.Metadata
            .FindProperty(nameof(ISoftDeletableEntity.IsDeleted))
            ?.GetColumnName();

        return !string.IsNullOrWhiteSpace(isDeletedColumnName) &&
            changes.Any(c => c.ColumnName.Equals(isDeletedColumnName, StringComparison.OrdinalIgnoreCase)
                    && c.NewValue is true)
            ? AuditAction.SoftDelete
            : Enum.Parse<AuditAction>(eventEntry.Action);
    }
}
