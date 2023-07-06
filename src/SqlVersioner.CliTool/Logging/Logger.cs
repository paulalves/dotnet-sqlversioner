namespace SqlVersioner.CliTool.Logging
{
  using System;
  using SqlVersioner.Abstractions.Logging;

  /// <summary>
  /// Console logger.
  /// </summary>
  /// <remarks>
  /// <para>This class is used to log to the console.</para>
  /// </remarks>
  public class Logger : ILogger
  {
    private readonly int logVerbosity;

    /// <summary>
    ///  Initializes a new instance of the <see cref="Logger"/> class.
    /// </summary>
    /// <param name="logVerbosity"></param>
    public Logger(int logVerbosity = 2)
    {
      this.logVerbosity = logVerbosity;
    }
    
    /// <inheritdoc/>  
    public void Log(LogVerbosity verbosity, string message, params object[] args)
    {
      if ((int)verbosity > this.logVerbosity) return;
      try
      {
        if (verbosity == LogVerbosity.Minimal)
          Console.ForegroundColor = ConsoleColor.Green;
        else if (verbosity == LogVerbosity.Normal)
          Console.ForegroundColor = ConsoleColor.DarkCyan;
        else if (verbosity == LogVerbosity.Detailed)
          Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(message, args);
      }
      finally
      {
        Console.ResetColor();
      }
    }

    /// <inheritdoc/>
    public void LogMinimal(string message, params object[] args)
    {
      Log(LogVerbosity.Minimal, message, args);
    }

    /// <inheritdoc/>
    public void LogNormal(string message, params object[] args)
    {
      Log(LogVerbosity.Normal, message, args);
    }

    /// <inheritdoc/>
    public void LogDetailed(string message, params object[] args)
    {
      Log(LogVerbosity.Detailed, message, args);
    }
  }
}