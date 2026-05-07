using System;

namespace StackedDeck.Persistence.Template.Entities;

/// <summary>
/// Defines properties for entities that track creation and modification metadata.
/// </summary>
public interface IAuditableEntity
{
    /// <summary>
    /// Gets or sets the identifier of the user who created the entity.
    /// </summary>
    Guid CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the entity was created.
    /// </summary>
    DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who last modified the entity.
    /// </summary>
    Guid UpdatedBy { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the entity was last modified.
    /// </summary>
    DateTimeOffset UpdatedAt { get; set; }
}
