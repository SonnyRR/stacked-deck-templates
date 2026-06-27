using System;

using Microsoft.Extensions.DependencyInjection;

using Spectre.Console.Cli;

namespace StackedDeck.CLI.Template.DependencyInjection;

/// <inheritdoc/>
public class SpectreCliTypeRegistrar : ITypeRegistrar
{
    private readonly IServiceCollection builder;

    /// <inheritdoc/>
    public SpectreCliTypeRegistrar(IServiceCollection builder)
        => this.builder = builder;

    /// <inheritdoc/>
    public ITypeResolver Build() => new SpectreCliTypeResolver(builder.BuildServiceProvider());

    /// <inheritdoc/>
    public void Register(Type service, Type implementation)
        => builder.AddSingleton(service, implementation);

    /// <inheritdoc/>
    public void RegisterInstance(Type service, object implementation)
        => builder.AddSingleton(service, implementation);

    /// <inheritdoc/>
    public void RegisterLazy(Type service, Func<object> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        builder.AddSingleton(service, _ => factory());
    }

    /// <inheritdoc/>
    public void RegisterType(Type service, Type implementation)
        => builder.AddTransient(service, implementation);

    /// <inheritdoc/>
    public void Add(Type service, Type implementation)
        => builder.AddTransient(service, implementation);
}
