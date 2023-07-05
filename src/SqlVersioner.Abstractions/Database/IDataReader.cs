namespace SqlVersioner.Abstractions.Database
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;

  public interface IDataReader : IDisposable, IAsyncDisposable
  {
    bool MoveNext(); 
    ValueTask<bool> MoveNextAsync(CancellationToken cancelToken = default);
    
    string GetString();
    string GetString(int ordinal);
  }
}