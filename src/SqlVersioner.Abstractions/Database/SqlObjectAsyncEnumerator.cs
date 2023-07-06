namespace SqlVersioner.Abstractions.Database
{
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>
  /// SqlObjectAsyncEnumerator class.
  /// </summary>
  /// <remarks>
  /// <para>This class is used to enumerate a <see cref="SqlObject"/>.</para>
  /// </remarks>
  internal sealed class SqlObjectAsyncEnumerator : IAsyncEnumerator<SqlObject>
  {
    private readonly IDataReader reader;
    private readonly CancellationToken cancelToken;

    /// <summary>
    /// Constructor with arguments.
    /// </summary>
    /// <param name="reader">The <see cref="IDataReader"/> instance.</param>
    /// <param name="cancelToken">The cancellation token.</param>
    public SqlObjectAsyncEnumerator(IDataReader reader, CancellationToken cancelToken)
    {
      this.reader = reader;
      this.cancelToken = cancelToken;
    }
    
    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
      return reader.DisposeAsync();
    }

    /// <inheritdoc />
    public ValueTask<bool> MoveNextAsync()
    {
      return reader.MoveNextAsync(this.cancelToken);
    }

    /// <inheritdoc />
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