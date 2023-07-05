namespace SqlVersioner.Abstractions.Database
{
  using System.Collections.Generic;

  public static class SqlDataReaderExtensions
  {
    public static IAsyncEnumerable<SqlObject> AsAsyncEnumerable(this IDataReader reader)
    {
      return new SqlObjectAsyncEnumerable(reader);
    }
  }
}