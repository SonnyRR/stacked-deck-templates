using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using StackedDeck.Persistence.Template.Entities;

namespace StackedDeck.Persistence.Template.Configuration;

public class AuditLogEntityTypeConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.EntityId).HasMaxLength(450);
        builder.Property(e => e.EntityName).HasMaxLength(450);
        builder.Property(e => e.Action).HasMaxLength(50);
        builder.Property(e => e.Timestamp).IsRequired();
    }
}
