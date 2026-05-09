namespace StackedDeck.Persistence.Template;

/// <summary>
/// The actions for an audit log.
/// </summary>
public enum AuditAction
{
    Insert = 1,
    Update,
    Delete,
    SoftDelete
}
