namespace SqlVersioner.Abstractions.Database
{
  using System.Threading;
  using System.Threading.Tasks;
  using SqlVersioner.Abstractions.Logging;

  public interface ISqlConnectionFactory
  {
    ILogger Logger { get; }
    
    Task<ISqlConnection> CreateConnectionAsync(CancellationToken cancelToken = default);
  }
}