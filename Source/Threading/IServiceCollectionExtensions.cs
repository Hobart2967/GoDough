using GoDough.Composition.Extensions;
using GoDough.Runtime.LivecycleHooks;
using Microsoft.Extensions.DependencyInjection;

namespace GoDough.Threading {
  public static class IServiceCollectionExtensions {
    public static IServiceCollection AddThreadingUtilities(this IServiceCollection serviceCollection) {
      return serviceCollection
        .AddSingleton<Dispatcher>()
        .MapSingleton<IOnProcess, Dispatcher>();
    }
  }
}