using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logging
{
    public class FileLoggerProvider : ILoggerProvider
    {
        string filePath;
        public FileLoggerProvider(string filePath)
        {
            this.filePath = filePath;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(filePath);
        }

        public void Dispose()
        {
            
        }
    }
}
