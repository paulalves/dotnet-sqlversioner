namespace SqlVersioner.SqlServer
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using SqlVersioner.Abstractions.Database;
  using SqlVersioner.Abstractions.Logging;

  public class SqlConnectionFactory : ISqlConnectionFactory
  {
    private readonly string connectionString;
    private readonly ILogger logger;

    public SqlConnectionFactory(string connectionString, ILogger logger)
    {
      if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString), "The connection string cannot be null or empty.");
      if (logger == null) throw new ArgumentNullException(nameof(logger), "The logger cannot be null.");
      
      this.connectionString = connectionString;
      this.logger = logger;
    }

    public ILogger Logger
    {
      get { return this.logger; }
    }
    
    public async Task<ISqlConnection> CreateConnectionAsync(CancellationToken cancelToken = default)
    {
      if (string.IsNullOrEmpty(connectionString)) throw new InvalidOperationException("The connection string cannot be null or empty.");

      Logger.LogNormal("Creating connection...");
      
      var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
      
      await connection
        .OpenAsync(cancelToken)
        .ConfigureAwait(false);

      return new SqlConnection(connection, Logger);
    }
  }
}