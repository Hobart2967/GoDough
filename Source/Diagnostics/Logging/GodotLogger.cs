using Godot;
using Microsoft.Extensions.Logging;
using System;

namespace GoDough.Diagnostics.Logging;

public sealed class GodotLogger : ILogger
{
  private readonly string _name;

  public GodotLogger(string name) => (_name) = (name);

  public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

  public bool IsEnabled(LogLevel logLevel) => true;

  public void Log<TState>(
      LogLevel logLevel,
      EventId eventId,
      TState state,
      Exception? exception,
      Func<TState, Exception?, string> formatter)
  {
    Action<string> printer = null;
    switch (logLevel)
    {
      case LogLevel.Information:
      case LogLevel.Debug:
      case LogLevel.Trace:
      case LogLevel.Warning:
        printer = str => GD.Print(str);
        break;

      case LogLevel.Error:
      case LogLevel.Critical:
        printer = str => GD.PrintErr(str);
        break;
    }

    if (printer == null)
    {
      return;
    }

    printer(String.Format("[{0}] [{1}] [{2}] {3}",
      DateTime.Now.ToString(),
      Enum.GetName(typeof(LogLevel), logLevel),
      this._name,
      formatter(state, exception)));
  }
}