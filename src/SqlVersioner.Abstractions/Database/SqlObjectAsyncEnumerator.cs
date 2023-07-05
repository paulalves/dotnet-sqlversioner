namespace SqlVersioner.Abstractions.Database
{
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;

  internal sealed class SqlObjectAsyncEnumerator : IAsyncEnumerator<SqlObject>
  {
    private readonly IDataReader reader;
    private readonly CancellationToken cancelToken;

    public SqlObjectAsyncEnumerator(IDataReader reader, CancellationToken cancelToken)
    {
      this.reader = reader;
      this.cancelToken = cancelToken;
    }
    
    public ValueTask DisposeAsync()
    {
      return reader.DisposeAsync();
    }

    public ValueTask<bool> MoveNextAsync()
    {
      return reader.MoveNextAsync(this.cancelToken);
    }

    public SqlObject Current
    {
      get { 
        return new SqlObject(
          Schema:     reader.GetString(0),
          ObjectName: reader.GetString(1),
          Type:       reader.GetString(2), 
          Definition: reader.GetString(3)); 
      }
    }
  }
}