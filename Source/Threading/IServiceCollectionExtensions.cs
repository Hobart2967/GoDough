using GoDough.Composition.Extensions;
using GoDough.Runtime.LivecycleHooks;
using Microsoft.Extensions.DependencyInjection;

namespace GoDough.Threading; 
public static class IServiceCollectionExtensions {
    public static IServiceCollection AddThreadingUtilities(this IServiceCollection serviceCollection) => serviceCollection
        .AddSingleton<Dispatcher>()
        .MapSingleton<IOnProcess, Dispatcher>();
}