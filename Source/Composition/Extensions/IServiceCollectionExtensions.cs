using Microsoft.Extensions.DependencyInjection;
using System;

namespace GoDough.Composition.Extensions;

public static class IServiceCollectionExtensions
{
  public static IServiceCollection MapSingleton<TNewMapping, TSource>(
    this IServiceCollection collection)
    where TSource : TNewMapping
    where TNewMapping : class => collection.AddSingleton<TNewMapping>(x => x.GetRequiredService<TSource>());

  public static IServiceCollection MapSingleton<TNewMapping>(
    this IServiceCollection collection, Type type)
    where TNewMapping : class => collection.AddSingleton<TNewMapping>(x => x.GetRequiredService(type) as TNewMapping);

  public static IServiceCollection AddFactory<TService, TImplementation>(this IServiceCollection services)
    where TService : class
    where TImplementation : class, TService => services
      .AddTransient<TService, TImplementation>()
      .AddSingleton<Func<TService>>(x => () => x.GetService<TService>())
      .AddSingleton<Factory<TService>, Factory<TService>>();

  public static IServiceCollection AddFactory<TService>(this IServiceCollection services)
    where TService : class => services.AddFactory<TService, TService>();
}