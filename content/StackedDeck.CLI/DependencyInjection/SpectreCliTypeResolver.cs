using System;

using Spectre.Console.Cli;

namespace StackedDeck.CLI.Template.DependencyInjection;

/// <inheritdoc/>
internal sealed class SpectreCliTypeResolver : ITypeResolver
{
    private readonly IServiceProvider provider;

    public SpectreCliTypeResolver(IServiceProvider provider)
        => this.provider = provider ?? throw new ArgumentNullException(nameof(provider));

    /// <inheritdoc/>
    public object Resolve(Type type) => provider.GetService(type);
}
