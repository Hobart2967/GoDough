using Microsoft.Extensions.DependencyInjection;

namespace GoDough.Runtime; 
public static class IServiceCollectionExtensions {
    public static IServiceCollection AddGodotRuntimeUtilities(this IServiceCollection serviceCollection) => serviceCollection
        .AddSingleton<IGodotApi, GodotApi>();
}