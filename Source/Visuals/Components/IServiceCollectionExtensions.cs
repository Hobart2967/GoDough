using Microsoft.Extensions.DependencyInjection;

namespace GoDough.Visuals.Components; 
public static class IServiceCollectionExtensions {
    public static IServiceCollection AddComponentFactory(this IServiceCollection serviceCollection) => serviceCollection
        .AddSingleton<ComponentFactory>();
}