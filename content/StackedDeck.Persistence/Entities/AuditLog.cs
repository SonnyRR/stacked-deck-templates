using System;

using Audit.EntityFramework;

using StackedDeck.Persistence.Template.Enums;

namespace StackedDeck.Persistence.Template.Entities;

/// <summary>
/// Represents an audit log entry capturing changes to entities in the system.
/// </summary>
[AuditIgnore]
public class AuditLog
{
    /// <summary>
    /// Gets or sets the primary key identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the affected entity.
    /// </summary>
    public string EntityId { get; set; }

    /// <summary>
    /// Gets or sets the name of the affected entity type.
    /// </summary>
    public string EntityName { get; set; }

    /// <summary>
    /// Gets or sets the action performed (Insert, Update, Delete, SoftDelete).
    /// </summary>
    public AuditAction Action { get; set; }

    /// <summary>
    /// Gets or sets the serialized column values after the action.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Gets or sets the serialized changes for update actions.
    /// </summary>
    public string Delta { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the action occurred.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }
}
