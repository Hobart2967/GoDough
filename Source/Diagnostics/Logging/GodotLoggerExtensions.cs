using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

namespace GoDough.Diagnostics.Logging {
  public static class GodotLoggerExtensions {
    public static ILoggingBuilder AddGodotLogger(this ILoggingBuilder builder) {
      builder.AddConfiguration();

      builder.Services.TryAddEnumerable(
          ServiceDescriptor.Singleton<ILoggerProvider, GodotLoggerProvider>());

      return builder;
    }
  }
}