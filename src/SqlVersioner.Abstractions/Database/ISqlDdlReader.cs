namespace SqlVersioner.Abstractions.Database
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;

  public interface ISqlDdlReader : IAsyncDisposable
  {
    Task<IReadOnlyList<SqlObject>> OpenSqlSchemasAsync(CancellationToken cancelToken = default);
    Task<IReadOnlyList<SqlObject>> OpenSqlTablesAsync(CancellationToken cancelToken = default);
    Task<IReadOnlyList<SqlObject>> OpenSqlFunctionsAsync(CancellationToken cancelToken = default);
    Task<IReadOnlyList<SqlObject>> OpenSqlProceduresAsync(CancellationToken cancelToken = default);
    Task<IReadOnlyList<SqlObject>> OpenSqlViewsAsync(CancellationToken cancelToken = default);
  }
}