namespace SqlVersioner.CliTool.Logging
{
  using System;
  using SqlVersioner.Abstractions.Logging;

  public class Logger : ILogger
  {
    private readonly int logVerbosity;

    public Logger(int logVerbosity = 2)
    {
      this.logVerbosity = logVerbosity;
    }
    
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

    public void LogMinimal(string message, params object[] args)
    {
      Log(LogVerbosity.Minimal, message, args);
    }

    public void LogNormal(string message, params object[] args)
    {
      Log(LogVerbosity.Normal, message, args);
    }

    public void LogDetailed(string message, params object[] args)
    {
      Log(LogVerbosity.Detailed, message, args);
    }
  }
}