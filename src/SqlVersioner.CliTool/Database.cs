namespace SqlVersioner.CliTool
{
  using System;
  using System.IO;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using Microsoft.Data.SqlClient;
  using SqlVersioner.Abstractions.Logging;
  using SqlVersioner.CliTool.Logging;
  using SqlVersioner.SqlServer;

  public class Database
  {
    private readonly SqlConnectionFactory factory;
    private readonly SqlConnectionStringBuilder builder;
    private readonly ILogger logger;
    
    public Database(string connectionString, int verbosity = 2)
    {
      this.logger = new Logger(verbosity);
      this.builder = new SqlConnectionStringBuilder(connectionString);
      this.factory = new SqlConnectionFactory(connectionString, Logger);
    }

    public ILogger Logger
    {
      get { return this.logger; }
    }

    public static async ValueTask WriteAsync(string connectionString, string path, int verbosity, CancellationToken cancelToken = default)
    {
      var database = new Database(connectionString, verbosity);
      await database.WriteAsync(path, cancelToken);
    }
    
    private async ValueTask WriteAsync(string path, CancellationToken cancelToken = default)
    {
      if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path), "The path cannot be null, empty, or whitespace.");
      cancelToken.ThrowIfCancellationRequested();
      
      Logger.LogMinimal("==== Writing DDL to {0}", path);
      await using (var reader = new SqlDdlReader(factory))
      {
        Logger.LogMinimal("==== Writing Schemas to {0}", path);
        var schemas = await reader.OpenSqlSchemasAsync(cancelToken).ConfigureAwait(false);
        foreach (var schema in schemas)
        {
          var directory = Directory.CreateDirectory(Path.Combine(path, builder.DataSource, builder.InitialCatalog, "schemas"));
          var script = Path.Combine(directory.FullName, $"{schema.ObjectName}.sql");
          await File.WriteAllTextAsync(script, schema.ToString(), Encoding.UTF8, cancelToken);    
        }
        
        Logger.LogMinimal("==== Writing Tables to {0}", path);
        foreach (var tables in await reader.OpenSqlTablesAsync(cancelToken).ConfigureAwait(false))
        {
          var directory = Directory.CreateDirectory(Path.Combine(path, builder.DataSource, builder.InitialCatalog, "tables"));
          var table = Path.Combine(directory.FullName, $"{tables.ObjectName}.sql");
          await File.WriteAllTextAsync(table, tables.ToString(), Encoding.UTF8, cancelToken);    
        }
        
        Logger.LogMinimal("==== Writing Functions to {0}", path);
        foreach (var functions in await reader.OpenSqlFunctionsAsync(cancelToken).ConfigureAwait(false))
        {
          var directory = Directory.CreateDirectory(Path.Combine(path, builder.DataSource, builder.InitialCatalog, "functions"));
          var function = Path.Combine(directory.FullName, $"{functions.ObjectName}.sql");
          await File.WriteAllTextAsync(function, functions.ToString(), Encoding.UTF8, cancelToken);    
        }
        
        Logger.LogMinimal("==== Writing Procedures to {0}", path);
        foreach (var procedures in await reader.OpenSqlProceduresAsync(cancelToken).ConfigureAwait(false))
        {
          var directory = Directory.CreateDirectory(Path.Combine(path, builder.DataSource, builder.InitialCatalog, "procedures"));
          var procedure = Path.Combine(directory.FullName, $"{procedures.ObjectName}.sql");
          await File.WriteAllTextAsync(procedure, procedures.ToString(), Encoding.UTF8, cancelToken);    
        }
        
        Logger.LogMinimal("==== Writing Views to {0}", path);
        foreach (var views in await reader.OpenSqlViewsAsync(cancelToken).ConfigureAwait(false))
        {
          var directory = Directory.CreateDirectory(Path.Combine(path, builder.DataSource, builder.InitialCatalog, "views"));
          var view = Path.Combine(directory.FullName, $"{views.ObjectName}.sql");
          await File.WriteAllTextAsync(view, views.ToString(), Encoding.UTF8, cancelToken);    
        }
      }
    }
  }
}