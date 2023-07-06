namespace SqlVersioner.Abstractions.Logging
{
  /// <summary>
  /// Log verbosity.
  /// </summary>
  /// <remarks>
  /// <para>This enum is used to represent the verbosity of the log.</para>
  /// </remarks>
  public enum LogVerbosity
  {
    /// <summary>
    /// No logging.
    /// </summary>
    None = 0,
    /// <summary>
    /// Minimal logging.
    /// </summary>
    Minimal = 1,
    /// <summary>
    /// Normal logging.
    /// </summary>
    Normal =  2,
    /// <summary>
    /// Detailed logging.
    /// </summary>
    Detailed = 3
  }
}