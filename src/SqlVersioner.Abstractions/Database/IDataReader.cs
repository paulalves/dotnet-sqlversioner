namespace SqlVersioner.Abstractions.Database
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>
  /// Data reader
  /// </summary>
  public interface IDataReader : IDisposable, IAsyncDisposable
  {
    /// <summary>
    /// Checks if there are more rows to read
    /// </summary>
    /// <returns>True if there are more rows to read</returns>
    bool MoveNext(); 
    
    /// <summary>
    /// Checks if there are more rows to read
    /// </summary>
    /// <param name="cancelToken">Cancellation token</param>
    /// <returns>True if there are more rows to read</returns>
    ValueTask<bool> MoveNextAsync(CancellationToken cancelToken = default);
    
    /// <summary>
    /// Gets the string value of the column at zero-based ordinal
    /// </summary>
    /// <returns>The string value of the column at zero-based ordinal</returns>
    string GetString();
    
    /// <summary>
    /// Gets the string value of the column based on the ordinal value 
    /// </summary>
    /// <param name="ordinal">The ordinal value</param>
    /// <returns>The string value of the column based on the ordinal value</returns>
    string GetString(int ordinal);
  }
}