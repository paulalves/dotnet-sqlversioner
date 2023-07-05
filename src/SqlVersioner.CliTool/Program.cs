namespace SqlVersioner.CliTool
{
  using System;
  using System.Threading.Tasks;
  using Microsoft.Data.SqlClient;
  using SqlVersioner.Abstractions.Arguments;

  public static class Program
  {
    public static async Task<int> Main(string[] args)
    {
      var arguments = CliArguments.Parse(args);
      
      try
      {
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
          .WriteAsync(
            builder.ConnectionString, 
            arguments.Output, arguments.Verbosity)
          .ConfigureAwait(false);
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