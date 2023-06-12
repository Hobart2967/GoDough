using System;
using Microsoft.Extensions.DependencyInjection;

namespace GoDough.Composition.Extensions {
  public static class IServiceCollectionExtensions {
    public static IServiceCollection MapSingleton<TNewMapping, TSource>(
      this IServiceCollection collection)
      where TSource : TNewMapping
      where TNewMapping : class {

      return collection.AddSingleton<TNewMapping>(x => x.GetRequiredService<TSource>());
    }

    public static IServiceCollection MapSingleton<TNewMapping>(
      this IServiceCollection collection, Type type)
      where TNewMapping : class {

      return collection.AddSingleton<TNewMapping>(x => x.GetRequiredService(type) as TNewMapping);
    }

    public static IServiceCollection AddFactory<TService, TImplementation>(this IServiceCollection services)
      where TService : class
      where TImplementation : class, TService {

      return services
        .AddTransient<TService, TImplementation>()
        .AddSingleton<Func<TService>>(x => () => x.GetService<TService>())
        .AddSingleton<Factory<TService>, Factory<TService>>();
    }

    public static IServiceCollection AddFactory<TService>(this IServiceCollection services)
      where TService : class {
      return services.AddFactory<TService, TService>();
    }
  }
}