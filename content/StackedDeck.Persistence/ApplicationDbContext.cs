using Microsoft.EntityFrameworkCore;

namespace StackedDeck.Persistence;

/// <summary>
/// The database context for the StackedDeck application.
/// </summary>
public class ApplicationDbContext : DbContext
{

    /// <summary>
    /// Initializes a new instance of the ApplicationDbContext.
    /// </summary>
    /// <param name="options">The options to configure the context.</param>
    public ApplicationDbContext(DbContextOptions options)
        : base(options)
    {
    }
}
