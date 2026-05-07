using System;

using Audit.EntityFramework;

namespace StackedDeck.Persistence.Template.Entities;

[AuditIgnore]
public class AuditLog
{
    public int Id { get; set; }

    public string EntityId { get; set; }

    public string EntityName { get; set; }

    public string Action { get; set; }

    public string Value { get; set; }

    public string Delta { get; set; }

    public DateTimeOffset Timestamp { get; set; }
}
