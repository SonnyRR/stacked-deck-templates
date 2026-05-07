using System;

namespace StackedDeck.Persistence.Template.Entities;

/// <summary>
/// Defines properties for entities that support soft deletion.
/// </summary>
public interface ISoftDeletableEntity
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity has been soft-deleted.
    /// </summary>
    bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who deleted the entity.
    /// </summary>
    Guid? DeletedBy { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the entity was deleted.
    /// </summary>
    DateTimeOffset? DeletedAt { get; set; }
}
