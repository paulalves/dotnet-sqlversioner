namespace SqlVersioner.CliTool
{
  using System;
  using System.Threading.Tasks;
  using Microsoft.Data.SqlClient;
  using SqlVersioner.Abstractions.Arguments;

  /// <summary>
  /// Program class.
  /// </summary>
  public static class Program
  {
    /// <summary>
    /// Main method.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>The exit code.</returns>
    public static async Task<int> Main(string[] args)
    {
      try
      {
        var arguments = CliArguments.Parse(args);
        var builder = new SqlConnectionStringBuilder
        {
          UserID = arguments.User,
          InitialCatalog = arguments.Database,
          DataSource = arguments.Server,
          Password = arguments.Password,
          Encrypt = true,
          TrustServerCertificate = true
        };

        await Database
          .WriteAsync(builder.ConnectionString,
            arguments.Output,
            arguments.Verbosity)
          .ConfigureAwait(false);
      }
      catch (CliArgumentException e)
      {
        Console.WriteLine(e.Message);
        Console.WriteLine();
        Console.WriteLine(CliArguments.GetUsage());
      }
      catch (Exception e)
      {
        Console.ForegroundColor = ConsoleColor.Red;
        await Console.Error.WriteLineAsync(e.Message);
        await Console.Error.WriteLineAsync(e.ToString());
        return 1;
      }
      finally
      {
        Console.ResetColor();
      }
      
      return 0;
    }
  }
}