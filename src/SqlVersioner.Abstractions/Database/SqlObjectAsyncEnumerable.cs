namespace SqlVersioner.Abstractions.Database
{
  using System;
  using System.Collections.Generic;
  using System.Threading;

  /// <summary>
  /// SQL object async enumerable.
  /// </summary>
  /// <remarks>
  public class SqlObjectAsyncEnumerable : IAsyncEnumerable<SqlObject>
  {
    private readonly IDataReader reader;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlObjectAsyncEnumerable"/> class.
    /// </summary>
    /// <param name="reader">The <see cref="IDataReader"/> instance.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="reader"/> cannot be null.</exception>
    /// <remarks>
    /// <para>This constructor is used to create an instance of the <see cref="SqlObjectAsyncEnumerable"/> class.</para>
    /// </remarks>
    public SqlObjectAsyncEnumerable(IDataReader reader)
    {
      if (reader == null) throw new ArgumentNullException(nameof(reader), "The reader cannot be null.");
      
      this.reader = reader;
    }
    
    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The <see cref="IAsyncEnumerator{T}"/> of <see cref="SqlObject"/>.</returns>
    public IAsyncEnumerator<SqlObject> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
      return new SqlObjectAsyncEnumerator(reader, cancellationToken);
    }
  }
}