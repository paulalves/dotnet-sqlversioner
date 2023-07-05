namespace SqlVersioner.Abstractions.Logging
{
  public interface ILogger
  {
    void Log(LogVerbosity verbosity, string message, params object[] args);

    void LogMinimal(string message, params object[] args);

    void LogNormal(string message, params object[] args);

    void LogDetailed(string message, params object[] args);
  }
}