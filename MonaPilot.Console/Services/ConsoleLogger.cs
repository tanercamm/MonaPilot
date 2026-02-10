using System;
using Microsoft.Extensions.Logging;

namespace MonaPilot.Console.Services
{
    public class ConsoleLogger : ILogger
    {
        private readonly string _categoryName;

        public ConsoleLogger(string categoryName)
        {
            _categoryName = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) where TState : notnull
        {
            var message = formatter(state, exception);
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            System.Console.WriteLine($"[{timestamp}] [{logLevel}] {_categoryName}: {message}");

            if (exception != null)
            {
                System.Console.WriteLine($"Exception: {exception}");
            }
        }
    }

    public class ConsoleLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName) => new ConsoleLogger(categoryName);
        public void Dispose() { }
    }
}
