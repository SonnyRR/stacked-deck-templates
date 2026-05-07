using System;

namespace StackedDeck.Persistence.Template.Entities;

public interface IAuditableEntity
{
    Guid CreatedBy { get; set; }

    DateTimeOffset CreatedAt { get; set; }

    Guid UpdatedBy { get; set; }

    DateTimeOffset UpdatedAt { get; set; }
}
