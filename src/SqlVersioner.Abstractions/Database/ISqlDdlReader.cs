namespace SqlVersioner.Abstractions.Database
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>
  ///  SQL DDL reader.
  /// </summary>
  /// <remarks>
  ///  <para>This interface is used to abstract the reading of SQL DDL objects.</para>
  /// </remarks> 
  public interface ISqlDdlReader : IAsyncDisposable
  {
    /// <summary>
    ///  Opens the SQL schemas.
    /// </summary>
    /// <param name="cancelToken">The cancellation token.</param>
    /// <returns>A task of the list of <see cref="SqlObject"/> instances.</returns>
    Task<IReadOnlyList<SqlObject>> OpenSqlSchemasAsync(CancellationToken cancelToken = default);
 
    /// <summary>
    ///  Opens the SQL tables.
    /// </summary>
    /// <param name="cancelToken">The cancellation token.</param>
    /// <returns>A task of the list of <see cref="SqlObject"/> instances.</returns>
    Task<IReadOnlyList<SqlObject>> OpenSqlTablesAsync(CancellationToken cancelToken = default);
    
    /// <summary>
    /// Opens the SQL functions.
    /// </summary>
    /// <param name="cancelToken">The cancellation token.</param>
    /// <returns>A task of the list of <see cref="SqlObject"/> instances.</returns>
    Task<IReadOnlyList<SqlObject>> OpenSqlFunctionsAsync(CancellationToken cancelToken = default);

    /// <summary>
    /// Opens the SQL procedures.
    /// </summary>
    /// <param name="cancelToken">The cancellation token.</param>
    /// <returns>A task of the list of <see cref="SqlObject"/> instances.</returns>
    Task<IReadOnlyList<SqlObject>> OpenSqlProceduresAsync(CancellationToken cancelToken = default);

    /// <summary>
    /// Opens the SQL views.
    /// </summary>
    /// <param name="cancelToken">The cancellation token.</param>
    /// <returns>A task of the list of <see cref="SqlObject"/> instances.</returns>
    Task<IReadOnlyList<SqlObject>> OpenSqlViewsAsync(CancellationToken cancelToken = default);
  }
}