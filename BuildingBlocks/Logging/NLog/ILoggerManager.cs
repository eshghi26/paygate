namespace Logging.NLog
{
    public interface ILoggerManager
    {
        void LogError(string message);
        void LogWarning(string message);
        void LogDebug(string message);
        void LogInfo(string message);
    }
}
