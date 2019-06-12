using System;

namespace Dbosoft.YaNco
{
    public interface ILogger
    {
        void LogException(Exception exception, string message);
        void LogException(Exception exception);
        void LogTrace(string message, object data);
        void LogError(string message, object data);

        void LogDebug(string message, object data);

        void LogTrace(string message);
        void LogDebug(string message);
        void LogError(string message);


    }
}