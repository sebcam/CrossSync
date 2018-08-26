using System.Threading;
using System.Threading.Tasks;
using CrossSync.Entity.Abstractions.UnitOfWork;
using Microsoft.EntityFrameworkCore.Storage;

namespace CrossSync.Entity.Abstractions.EF.UnitOfWork
{
  /// <summary>
  /// Unit of work interface using Entity Framework
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface IUnitOfWork<out T> : IUnitOfWork where T : IContext
  {
    /// <summary>
    /// Gets the data context
    /// </summary>
    T Context { get; }

    /// <summary>
    /// Start a new database transaction
    /// </summary>
    /// <param name="cancellationToken">A cancelation token</param>
    /// <returns>The created transaction.</returns>
    Task<IDbContextTransaction> BeginTransaction(CancellationToken cancellationToken = default(CancellationToken));
  }
}
