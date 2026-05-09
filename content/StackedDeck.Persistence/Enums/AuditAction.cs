namespace StackedDeck.Persistence.Template.Enums;

/// <summary>
/// The actions for an audit log.
/// </summary>
public enum AuditAction
{
    /// <summary>
    /// Represents an insert operation.
    /// </summary>
    Insert = 1,

    /// <summary>
    /// Represents an update operation.
    /// </summary>
    Update,

    /// <summary>
    /// Represents a delete operation.
    /// </summary>
    Delete,

    /// <summary>
    /// Represents a soft delete operation.
    /// </summary>
    SoftDelete
}
