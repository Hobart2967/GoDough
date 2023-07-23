using System;
using System.Collections.Concurrent;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;

namespace GoDough.Diagnostics.Logging; 

[UnsupportedOSPlatform("browser")]
[ProviderAlias("ColorConsole")]
public sealed class GodotLoggerProvider : ILoggerProvider
{
private readonly ConcurrentDictionary<string, GodotLogger> _loggers =
  new(StringComparer.OrdinalIgnoreCase);

public ILogger CreateLogger(string categoryName) =>
  _loggers.GetOrAdd(categoryName, name => new GodotLogger(name));

public void Dispose() {
  _loggers.Clear();
}
}