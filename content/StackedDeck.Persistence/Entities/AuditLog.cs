using System;
using Audit.EntityFramework;

namespace StackedDeck.Persistence.Template.Entities;

[AuditIgnore]
public class AuditLog
{
    public int Id { get; set; }
    public string EntityId { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Changes { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}
