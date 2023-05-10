using Microsoft.Extensions.DependencyInjection;

namespace GoDough.Visuals.Components {
  public static class IServiceCollectionExtensions {
    public static IServiceCollection AddComponentFactory(this IServiceCollection serviceCollection) {
      return serviceCollection
        .AddSingleton<ComponentFactory>();
    }
  }
}