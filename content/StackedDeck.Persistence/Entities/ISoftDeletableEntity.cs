using System;

namespace StackedDeck.Persistence.Template.Entities;

public interface ISoftDeletableEntity
{
    bool IsDeleted { get; set; }

    Guid? DeletedBy { get; set; }

    DateTimeOffset? DeletedAt { get; set; }
}
