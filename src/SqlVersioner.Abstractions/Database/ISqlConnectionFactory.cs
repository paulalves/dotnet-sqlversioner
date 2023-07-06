namespace SqlVersioner.Abstractions.Database
{
  using System.Threading;
  using System.Threading.Tasks;
  using SqlVersioner.Abstractions.Logging;

  /// <summary>
  ///  Factory for creating <see cref="ISqlConnection"/> instances.
  /// </summary>
  /// <remarks>
  ///  <para>This interface is used to abstract the creation of <see cref="ISqlConnection"/> instances.</para>
  ///  <para>Implementations of this interface should be thread-safe.</para>
  /// </remarks>
  public interface ISqlConnectionFactory
  {
    /// <summary>
    /// Gets the logger.
    /// </summary>
    ILogger Logger { get; }
    
    /// <summary>
    ///  Creates a new <see cref="ISqlConnection"/> instance.
    /// </summary>
    /// <param name="cancelToken">The cancellation token.</param>
    /// <returns>A task of the <see cref="ISqlConnection"/> instance.</returns>
    Task<ISqlConnection> CreateConnectionAsync(CancellationToken cancelToken = default);
  }
}