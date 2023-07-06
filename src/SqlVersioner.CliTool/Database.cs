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

  /// <summary>
  /// Database class.
  /// </summary>
  /// <remarks>
  /// <para>This class is used to write the SQL DDL objects to the file system.</para>
  /// </remarks>
  public class Database
  {
    private readonly SqlConnectionFactory factory;
    private readonly SqlConnectionStringBuilder builder;
    private readonly ILogger logger;
    
    /// <summary>
    /// Constructor with arguments.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="verbosity">The verbosity level.</param>
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

    /// <summary>
    /// Writes the SQL DDL objects to the file system.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="path">The path.</param>
    /// <param name="verbosity">The verbosity level.</param>
    /// <param name="cancelToken">The cancellation token.</param>
    /// <returns>Returns a <see cref="ValueTask"/> promise.</returns>
    /// <exception cref="ArgumentNullException">The path cannot be null, empty, or whitespace.</exception>
    /// <exception cref="DirectoryNotFoundException">The directory does not exist.</exception>
    /// <exception cref="IOException">An I/O error occurred while opening the file.</exception>
    /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission.</exception>
    /// <exception cref="OperationCanceledException">The operation was canceled.</exception> 
    public static async ValueTask WriteAsync(string connectionString, string path, int verbosity, CancellationToken cancelToken = default)
    {
      var database = new Database(connectionString, verbosity);
      await database.WriteAsync(path, cancelToken);
    }
    
    /// <summary>
    /// Writes the SQL DDL objects to the file system.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="cancelToken">The cancellation token.</param>
    /// <returns>Returns a <see cref="ValueTask"/> promise.</returns>
    /// <exception cref="ArgumentNullException">The path cannot be null, empty, or whitespace.</exception>
    /// <exception cref="DirectoryNotFoundException">The directory does not exist.</exception>
    /// <exception cref="IOException">An I/O error occurred while opening the file.</exception>
    /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission.</exception>
    /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
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