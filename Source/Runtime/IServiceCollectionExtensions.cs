using Microsoft.Extensions.DependencyInjection;

namespace GoDough.Runtime; 
public static class IServiceCollectionExtensions {
public static IServiceCollection AddGodotRuntimeUtilities(this IServiceCollection serviceCollection) {
  return serviceCollection
    .AddSingleton<IGodotApi, GodotApi>();
}
}