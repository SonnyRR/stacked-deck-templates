using System.Linq;
using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

using StackedDeck.Persistence.Template.Entities;

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
        ApplySoftDeleteQueryFilters(modelBuilder);
    }

    /// <summary>
    /// Applies a global query filter to exclude soft-deleted entities from all queries.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    private static void ApplySoftDeleteQueryFilters(ModelBuilder modelBuilder)
    {
        var softDeletableEntityTypes = modelBuilder
            .Model
            .GetEntityTypes()
            .Where(met => typeof(ISoftDeletableEntity).IsAssignableFrom(met.ClrType))
            .Select(met => met.ClrType);

        foreach (var entityType in softDeletableEntityTypes)
        {
            var parameter = Expression.Parameter(entityType, "e");
            var property = Expression.Property(parameter, nameof(ISoftDeletableEntity.IsDeleted));
            var comparison = Expression.Equal(property, Expression.Constant(false));
            var lambda = Expression.Lambda(comparison, parameter);

            modelBuilder
                .Entity(entityType)
                .HasQueryFilter(Constants.GlobalQueryFilters.SOFT_DELETE, lambda);
        }
    }
}
