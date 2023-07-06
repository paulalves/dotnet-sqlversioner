namespace SqlVersioner.Abstractions.Database
{
  using System.Collections.Generic;

  /// <summary>
  /// SQL data reader extensions.
  /// </summary>
  public static class SqlDataReaderExtensions
  {
    /// <summary>
    /// Converts the <see cref="IDataReader"/> to an <see cref="IAsyncEnumerable{T}"/> of <see cref="SqlObject"/>.
    /// </summary>
    /// <param name="reader">The <see cref="IDataReader"/> instance.</param>
    /// <returns>The <see cref="IAsyncEnumerable{T}"/> of <see cref="SqlObject"/>.</returns>
    public static IAsyncEnumerable<SqlObject> AsAsyncEnumerable(this IDataReader reader)
    {
      return new SqlObjectAsyncEnumerable(reader);
    }
  }
}