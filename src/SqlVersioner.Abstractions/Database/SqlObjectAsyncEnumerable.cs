namespace SqlVersioner.Abstractions.Database
{
  using System;
  using System.Collections.Generic;
  using System.Threading;

  public class SqlObjectAsyncEnumerable : IAsyncEnumerable<SqlObject>
  {
    private readonly IDataReader reader;

    public SqlObjectAsyncEnumerable(IDataReader reader)
    {
      if (reader == null) throw new ArgumentNullException(nameof(reader), "The reader cannot be null.");
      
      this.reader = reader;
    }
    
    public IAsyncEnumerator<SqlObject> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
      return new SqlObjectAsyncEnumerator(reader, cancellationToken);
    }
  }
}