using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using StackedDeck.Persistence.Template.Entities;

namespace StackedDeck.Persistence.Template.Configuration;

/// <summary>
/// Configures the entity type mapping for <see cref="AuditLog"/>.
/// </summary>
public class AuditLogEntityTypeConfiguration : IEntityTypeConfiguration<AuditLog>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.EntityId).HasMaxLength(450);
        builder.Property(e => e.EntityName).HasMaxLength(450);
        builder.Property(e => e.Action).HasConversion<string>();
        builder.Property(e => e.Value).IsRequired(true);
        builder.Property(e => e.Delta).IsRequired(false);
        builder.Property(e => e.Timestamp).IsRequired();
        builder.HasIndex(e => new { e.EntityId, e.EntityName, e.Timestamp });
    }
}
