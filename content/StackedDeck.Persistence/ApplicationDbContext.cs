using Microsoft.EntityFrameworkCore;

#if (UseAuditNet)
using Audit.EntityFramework;
#endif

#if (UseAuditNet && !UseSeparateAuditTables)
using StackedDeck.Persistence.Template.Entities;
#endif

namespace StackedDeck.Persistence.Template;

/// <summary>
/// The database context for the StackedDeck application.
/// </summary>
public class ApplicationDbContext : DbContext
{
#if (UseAuditNet && !UseSeparateAuditTables)
    public DbSet<AuditLog> AuditLogs { get; set; }
#endif

    /// <summary>
    /// Initializes a new instance of the ApplicationDbContext.
    /// </summary>
    /// <param name="options">The options to configure the context.</param>
    public ApplicationDbContext(DbContextOptions options)
        : base(options)
    {
    }

#if (UseAuditNet)
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new AuditSaveChangesInterceptor());
    }
#endif

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
