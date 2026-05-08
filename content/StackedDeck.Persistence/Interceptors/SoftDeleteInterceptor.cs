using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

using StackedDeck.Persistence.Template.Entities;

namespace StackedDeck.Persistence.Template.Interceptors;

/// <summary>
/// Custom interceptor that attaches soft-deletion metadata to entities that implement
/// <see cref="ISoftDeletableEntity"/> before SaveChanges() and SaveChangesAsync() are invoked.
/// </summary>
public class SoftDeleteInterceptor : SaveChangesInterceptor
{
    /// <inheritdoc/>
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
        {
            SetSoftDeletedMetadata(eventData.Context);
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
            SetSoftDeletedMetadata(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// Sets the <see cref="ISoftDeletableEntity.IsDeleted"/> flag to <c>true</c>
    /// for any entities that have the <see cref="EntityState.Deleted"/> state.
    /// </summary>
    private static void SetSoftDeletedMetadata(DbContext dbContext)
    {
        var deletedEntries = dbContext
            .ChangeTracker
            .Entries<ISoftDeletableEntity>()
            .Where(e => e.State is EntityState.Deleted);

        // NOTE: You can potentially use an 'IMachineDateTime' interface,
        // so that you can have more fine control over the datetime providers
        // especially in unit/integration tests.
        var utcNow = DateTimeOffset.UtcNow;

        // TODO: Implement a custom 'IUserContext' which should be a dependency
        // for this interceptor. This interface & implementations should reside
        // in a common/shared kernel assembly. As a default - the value is set
        // to an empty GUID.
        var userId = Guid.Empty;

        foreach (var entry in deletedEntries)
        {
            var entity = entry.Entity;
            entity.IsDeleted = true;
            entity.DeletedAt = utcNow;
            entity.DeletedBy = userId;

            entry.State = EntityState.Modified;
        }
    }
}
