using NLog;
using Services.Contracts;

namespace Services;
public class LoggerManager : ILoggerService {
    private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

    public void LogDebug(String message) {
        logger.Debug(message);
    }

    public void LogError(String message) {
        logger.Error(message);
    }

    public void LogInfo(String message) {
        logger.Info(message);
    }

    public void LogWarning(String message) {
        logger.Warn(message);
    }
}