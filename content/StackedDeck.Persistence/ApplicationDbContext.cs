using Microsoft.EntityFrameworkCore;

#if (UseAuditNet)
using StackedDeck.Persistence.Template.Entities;
#endif

namespace StackedDeck.Persistence.Template;

/// <summary>
/// The application's database context.
/// </summary>
public class ApplicationDbContext : DbContext
{
#if (UseAuditNet)
    public DbSet<AuditLog> AuditLogs { get; set; }
#endif

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationDbContext"/>.
    /// </summary>
    /// <param name="options">The options to configure the context.</param>
    public ApplicationDbContext(DbContextOptions options)
        : base(options)
    {
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
