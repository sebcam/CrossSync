using System.Threading.Tasks;
using CrossSync.Entity.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CrossSync.Infrastructure.Client
{
  /// <summary>
  /// Client side database context interface
  /// </summary>
  public interface IClientContext : IContext
  {
    /// <summary>
    /// Gets the operations set
    /// </summary>
    DbSet<Operation> Operations { get; }

    /// <summary>
    /// Commit changes
    /// </summary>
    /// <param name="bypassOperations">True to do not track changes into Operation DbSet. Default is False</param>
    /// <returns></returns>
    Task<int> CommitAsync(bool bypassOperations = false);
  }
}