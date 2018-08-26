using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace CrossSync.Entity.Abstractions
{
  internal class ContextLogger : ILoggerProvider
  {
    public ILogger CreateLogger(string categoryName)
    {
      return new MyLogger();
    }

    public void Dispose()
    { }

    private class MyLogger : ILogger
    {
      public bool IsEnabled(LogLevel logLevel)
      {
        return true;
      }

      public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
      {
        if (eventId.Id == 200101)
        {
          Debug.WriteLine(formatter(state, exception));
          Trace.WriteLine(formatter(state, exception));
        }
      }

      public IDisposable BeginScope<TState>(TState state)
      {
        return null;
      }
    }
  }
}
