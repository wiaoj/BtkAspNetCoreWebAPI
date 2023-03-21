namespace Services.Contracts;
public interface ILoggerService {
    void LogInfo(String message);
    void LogWarning(String message);
    void LogError(String message);
    void LogDebug(String message);
}