namespace SqlVersioner.SqlServer
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using Microsoft.Data.SqlClient;
  using SqlVersioner.Abstractions.Database;
  using SqlVersioner.Abstractions.Logging;

  /// <summary>
  /// <see cref="IDataReader"/> implementation for SQL Server.
  /// </summary>
  /// <remarks>
  ///  <para>This class is used to read data from a SQL Server database.</para>
  /// </remarks>
  public class SqlDataReader : IDataReader
  {
    private readonly SqlCommand command;
    private readonly SqlTransaction transaction;
    private readonly ILogger logger;
    private Microsoft.Data.SqlClient.SqlDataReader dataReader;

    /// <summary>
    /// Constructor with arguments.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="transaction">The transaction.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="System.ArgumentNullException">The command cannot be null.</exception>
    /// <exception cref="System.ArgumentNullException">The transaction cannot be null.</exception>
    /// <exception cref="System.ArgumentNullException">The logger cannot be null.</exception>
    public SqlDataReader(
      SqlCommand command, 
      SqlTransaction transaction, 
      ILogger logger)
    {
      if (command == null) throw new ArgumentNullException(nameof(command), "The command cannot be null.");
      if (transaction == null) throw new ArgumentNullException(nameof(transaction), "The transaction cannot be null.");
      if (logger == null) throw new ArgumentNullException(nameof(logger), "The logger cannot be null.");
      
      this.command = command;
      this.transaction = transaction;
      this.logger = logger;
    }

    public ILogger Logger
    {
      get { return this.logger; }
    }
    
    /// <summary>
    /// Executes the reader.
    /// </summary>
    /// <returns>The <see cref="IDataReader"/>.</returns>
    public IDataReader ExecuteReader()
    {
      dataReader = command.ExecuteReader();
      return this;
    }

    /// <summary>
    /// Executes the reader asynchronously.
    /// </summary>
    /// <param name="cancelToken">The cancellation token.</param>
    /// <returns>The <see cref="IDataReader"/>.</returns>
    public async ValueTask<IDataReader> ExecuteReaderAsync(CancellationToken cancelToken = default)
    {
      Logger.LogDetailed("Executing Reader");
      
      dataReader = await command
        .ExecuteReaderAsync(cancelToken)
        .ConfigureAwait(false);

      return this;
    }
    
    /// <inheritdoc />
    public bool MoveNext()
    {
      return dataReader.Read();
    }

    /// <inheritdoc />
    public string GetString(int ordinal) => dataReader.GetString(ordinal);

    /// <inheritdoc />
    public string GetString() => GetString(0);

    /// <inheritdoc />
    public async ValueTask<bool> MoveNextAsync(CancellationToken cancelToken = default)
    {
      Logger.LogDetailed("Executing MoveNextAsync.");
      
      return await dataReader
        .ReadAsync(cancelToken)
        .ConfigureAwait(false);
    }
    
    /// <inheritdoc />
    public void Dispose()
    {
      if (isDisposed) return;

      isDisposed = true; 

      Logger.LogDetailed("Disposing Reader.");

      if (!dataReader.IsClosed)
      {
        this.dataReader.Close();
      }
      
      this.dataReader.Dispose();
      this.transaction.Dispose();
      this.command.Dispose(); 
    }

    private bool isDisposed;
    
    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
      if (isDisposed) return;

      isDisposed = true; 
      
      if (!dataReader.IsClosed)
      {
        await this.dataReader.CloseAsync();
      }
      
      await this.dataReader.DisposeAsync();
      await this.transaction.DisposeAsync();
      await this.command.DisposeAsync(); 
    }
  }
}