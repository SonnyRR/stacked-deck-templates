using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackedDeck.Persistence.Template.Configuration;

/// <summary>
/// Base configuration for temporal (history-tracking) entity types.
/// </summary>
/// <typeparam name="T">The entity type to configure.</typeparam>
public abstract class TemporalEntityTypeConfiguration<T> : IEntityTypeConfiguration<T>
    where T : class
{
    /// <inheritdoc />
    public virtual void Configure(EntityTypeBuilder<T> builder)
        => builder.ToTable(typeof(T).Name, tb => tb.IsTemporal());
}
