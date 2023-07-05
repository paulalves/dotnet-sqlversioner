namespace SqlVersioner.SqlServer
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using Microsoft.Data.SqlClient;
  using SqlVersioner.Abstractions.Database;
  using SqlVersioner.Abstractions.Logging;

  public class SqlConnection : ISqlConnection
  {
    private readonly Microsoft.Data.SqlClient.SqlConnection connection;
    private readonly ILogger logger;
    private bool isDisposed;

    public SqlConnection(Microsoft.Data.SqlClient.SqlConnection connection, ILogger logger)
    {
      if (connection == null) throw new ArgumentNullException(nameof(connection), "The connection cannot be null.");
      if (logger == null) throw new ArgumentNullException(nameof(logger), "The logger cannot be null.");

      this.connection = connection;
      this.logger = logger;
    }

    public ILogger Logger
    {
      get { return this.logger; }
    }

    public async Task<T> ExecuteScalarAsync<T>(string sqlCommand, CancellationToken cancelToken = default)
    {
      if (string.IsNullOrEmpty(sqlCommand)) throw new ArgumentNullException(nameof(sqlCommand), "The sql command cannot be null or empty.");
      
      Logger.LogDetailed("Executing scalar command: {0}", sqlCommand);
      
      var (command, transaction) = await PrepareAsync(sqlCommand, cancelToken).ConfigureAwait(false);
      await using (command)
      await using (transaction)
      {
        try
        {
          var obj = await command.ExecuteScalarAsync(cancelToken).ConfigureAwait(false);
          if (obj is T scalar)
            return scalar;
          else
            return default!;
        }
        finally
        {
          await transaction
            .CommitAsync(cancelToken)
            .ConfigureAwait(false);
        }
      }
    }

    public async Task<System.Data.DataTable> ExecuteQueryAsync(string sqlCommand, CancellationToken cancelToken = default)
    {
      if (string.IsNullOrEmpty(sqlCommand)) throw new ArgumentNullException(nameof(sqlCommand), "The sql command cannot be null or empty.");

      Logger.LogDetailed("Executing query command: {0}", sqlCommand);
      
      var (command, transaction) = await PrepareAsync(sqlCommand, cancelToken).ConfigureAwait(false);
      await using (command)
      await using (transaction)
      {
        using (var adapter = new SqlDataAdapter(command))
        {
          Logger.LogNormal("Filling data table...");
          
          var table = new System.Data.DataTable();
          adapter.Fill(table);
          
          await transaction
            .CommitAsync(cancelToken)
            .ConfigureAwait(false);
          
          return table;
        }
      }
    }

    public async Task<int> ExecuteNonQueryAsync(string sqlCommand, CancellationToken cancelToken = default)
    {
      if (string.IsNullOrEmpty(sqlCommand)) throw new ArgumentNullException(nameof(sqlCommand), "The sql command cannot be null or empty.");

      Logger.LogDetailed("Executing non-query command: {0}", sqlCommand);
      
      var (command, transaction) = await PrepareAsync(sqlCommand, cancelToken).ConfigureAwait(false);

      await using (command)
      await using (transaction)
      {
        var affectedRows = await command
          .ExecuteNonQueryAsync(cancelToken)
          .ConfigureAwait(false);

        await transaction
          .CommitAsync(cancelToken)
          .ConfigureAwait(false);

        return affectedRows;
      }
    }

    public async Task<IDataReader> ExecuteReaderAsync(string sqlCommand, CancellationToken cancelToken = default)
    {
      if (string.IsNullOrEmpty(sqlCommand)) throw new ArgumentNullException(nameof(sqlCommand), "The sql command cannot be null or empty.");
      
      Logger.LogDetailed("Executing reader command: {0}", sqlCommand);
      
      var (command, transaction) = await PrepareAsync(sqlCommand, cancelToken).ConfigureAwait(false);

      var reader = new SqlDataReader(command, transaction, Logger);
      
      Logger.LogNormal("Returning reader...");
      
      return reader.ExecuteReader();
    }

    private async Task<(SqlCommand, SqlTransaction)> PrepareAsync(string sqlCommand, CancellationToken cancelToken = default)
    {
      Logger.LogDetailed("Preparing command: {0}", sqlCommand);
      
      var command = this.connection.CreateCommand();
      var transaction = (SqlTransaction)await this.connection
        .BeginTransactionAsync(cancelToken)
        .ConfigureAwait(false);

      command.CommandText = sqlCommand;
      command.Connection = this.connection;
      //command.CommandType = CommandType.Text;
      command.Transaction = transaction;

      return (command, transaction);
    }
    
    public void Dispose()
    {
      if (this.isDisposed) return;
      
      this.isDisposed = true; 
      
      if (connection == null) return;
      
      this.connection.Dispose();
    }

    public ValueTask DisposeAsync()
    {
      if (this.isDisposed) return ValueTask.CompletedTask;
      
      this.isDisposed = true; 
      
      return this.connection.DisposeAsync();
    }
  }
}