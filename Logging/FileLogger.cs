using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging
{
    public class FileLogger : ILogger, IDisposable
    {
        string filePath;
        object _lock = new object();
        public FileLogger(string filePath)
        {
            this.filePath = filePath;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public void Dispose()
        {
            
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("LogLevel: {6}{7}\tTime: {0},{1}\tException: {2},{3}\tState: {4}{5}", DateTime.Now, Environment.NewLine, exception.ToString(), Environment.NewLine, state.ToString(), Environment.NewLine, logLevel, Environment.NewLine);
            if (formatter != null)
            {
                lock(_lock)
                {
                    File.AppendAllText(filePath, stringBuilder + formatter(state, exception) + Environment.NewLine);
                }
            }
            else
            {
                lock(_lock)
                {
                    File.AppendAllText(filePath, stringBuilder.ToString());
                }
            }
        }
    }
}
