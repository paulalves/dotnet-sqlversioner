namespace SqlVersioner.Abstractions.Database
{
  using System;
  using System.Data;
  using System.Threading;
  using System.Threading.Tasks;

  public interface ISqlConnection : IDisposable, IAsyncDisposable
  {
    Task<T> ExecuteScalarAsync<T>(string sqlCommand, CancellationToken cancelToken = default);

    Task<DataTable> ExecuteQueryAsync(string sqlCommand, CancellationToken cancelToken = default);

    Task<int> ExecuteNonQueryAsync(string sqlCommand, CancellationToken cancelToken = default);
    
    Task<IDataReader> ExecuteReaderAsync(string sqlCommand, CancellationToken cancelToken = default);
  }
}