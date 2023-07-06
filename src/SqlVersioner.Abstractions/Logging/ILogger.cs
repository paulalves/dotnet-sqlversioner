namespace SqlVersioner.Abstractions.Logging
{
  /// <summary>
  /// Logger interface
  /// </summary>
  public interface ILogger
  {
    /// <summary>
    /// Logs a message.
    /// </summary>
    /// <param name="verbosity">The verbosity.</param>
    /// <param name="message">The message.</param>
    /// <param name="args">The arguments.</param>
    void Log(LogVerbosity verbosity, string message, params object[] args);

    /// <summary>
    /// Logs a minimal message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="args">The arguments.</param>
    void LogMinimal(string message, params object[] args);

    /// <summary>
    /// Logs a normal message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="args">The arguments.</param>
    void LogNormal(string message, params object[] args);
    
    /// <summary>
    /// Logs a detailed message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="args">The arguments.</param>
    void LogDetailed(string message, params object[] args);
  }
}