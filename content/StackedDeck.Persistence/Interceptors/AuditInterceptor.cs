using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#if (UseAuditNet)
using Audit.EntityFramework;

#endif
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

using StackedDeck.Persistence.Template.Entities;

namespace StackedDeck.Persistence.Template.Interceptors;

#if (UseAuditNet)
/// <summary>
/// Custom interceptor, that extends <see cref="AuditSaveChangesInterceptor"/>,
/// and attaches auditable metadata to entities that implement <see cref="IAuditableEntity"/>
/// before SaveChanges() and SaveChangesAsync() are invoked.
/// </summary>
/// <remarks>
/// This interceptor extends the audit interceptor from Audit.NET, which will store snapshots
/// of the entities & their deltas in a dedicated audit log table. This behavior will be retained,
/// but the sole purpose of this interceptor is to attach the custom auditable metadata, part of the
/// <see cref="IAuditableEntity"/> interface to the current active record.
/// </remarks>
public class AuditInterceptor : AuditSaveChangesInterceptor
#else
/// <summary>
/// Custom interceptor that attaches auditable metadata to entities that implement
/// <see cref="IAuditableEntity"/> before SaveChanges() and SaveChangesAsync() are invoked.
/// </summary>
public class AuditInterceptor : SaveChangesInterceptor
#endif
{
    /// <inheritdoc/>
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
        {
            SetAuditMetadata(eventData.Context);
        }

        return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc/>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            SetAuditMetadata(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// Sets audit metadata for all entities that implement <see cref="IAuditableEntity"/>.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    private static void SetAuditMetadata(DbContext dbContext)
    {
        var entries = dbContext
            .ChangeTracker
            .Entries<IAuditableEntity>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified);

        // NOTE: You can potentially use an 'IMachineDateTime' interface,
        // so that you can have more fine control over the datetime providers
        // especially in unit/integration tests.
        var utcNow = DateTimeOffset.UtcNow;

        // TODO: Implement a custom 'IUserContext' which should be a dependency
        // for this interceptor. This interface & implementations should reside
        // in a common/shared kernel assembly. As a default - the value is set
        // to an empty GUID.
        var userId = Guid.Empty;

        foreach (var entry in entries)
        {
            if (entry.State is EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;
                entry.Entity.CreatedBy = userId;
            }
            else if (entry.State is EntityState.Modified)
            {
                entry.Entity.UpdatedAt = utcNow;
                entry.Entity.UpdatedBy = userId;
            }
        }
    }
}
