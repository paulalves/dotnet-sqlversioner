namespace SqlVersioner.Abstractions.Database
{
  using System;
  using System.Data;
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>
  /// SQL connection
  /// <remarks>This interface is a wrapper around the <see cref="System.Data.SqlClient.SqlConnection"/> class</remarks>
  /// </summary>
  public interface ISqlConnection : IDisposable, IAsyncDisposable
  {
    /// <summary>
    /// Executes a scalar query asynchronously
    /// </summary>
    /// <param name="sqlCommand">The SQL command to execute</param>
    /// <param name="cancelToken">The cancellation token</param>
    /// <typeparam name="T">The type of the scalar value</typeparam>
    /// <returns>Task of the scalar value</returns>
    /// <remarks>This method is used for SELECT statements that return a single value</remarks>
    Task<T> ExecuteScalarAsync<T>(string sqlCommand, CancellationToken cancelToken = default);

    /// <summary>
    /// Executes a query asynchronously
    /// </summary>
    /// <param name="sqlCommand">The SQL command to execute</param>
    /// <param name="cancelToken">The cancellation token</param>
    /// <returns>Task of the data table</returns>
    /// <remarks>This method is used for SELECT statements</remarks>
    Task<DataTable> ExecuteQueryAsync(string sqlCommand, CancellationToken cancelToken = default);

    /// <summary>
    /// Executes a non-query asynchronously
    /// </summary>
    /// <param name="sqlCommand">The SQL command to execute</param>
    /// <param name="cancelToken">The cancellation token</param>
    /// <returns>Task of the number of rows affected</returns>
    /// <remarks>This method is used for INSERT, UPDATE, DELETE, and DDL statements</remarks>
    Task<int> ExecuteNonQueryAsync(string sqlCommand, CancellationToken cancelToken = default);

    /// <summary>
    /// Executes a reader asynchronously
    /// </summary>
    /// <param name="sqlCommand">The SQL command to execute</param>
    /// <param name="cancelToken">The cancellation token</param>
    /// <returns>Task of the data reader</returns>
    /// <remarks>
    ///   <para>The caller is responsible for disposing the data reader</para>
    ///   <para>This method is used for SELECT statements</para>
    /// </remarks>
    Task<IDataReader> ExecuteReaderAsync(string sqlCommand, CancellationToken cancelToken = default);
  }
}