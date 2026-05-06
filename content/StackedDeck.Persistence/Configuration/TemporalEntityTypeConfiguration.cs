using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StackedDeck.Persistence.Template.Configuration;

public abstract class TemporalEntityTypeConfiguration<T> : IEntityTypeConfiguration<T>
    where T : class
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
        => builder.ToTable(typeof(T).Name, tb => tb.IsTemporal());
}
