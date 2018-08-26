using System.Threading;
using System.Threading.Tasks;
using CrossSync.Entity.Abstractions.EF.UnitOfWork;
using Microsoft.EntityFrameworkCore.Storage;

namespace CrossSync.Infrastructure.Client.UnitOfWork
{
  /// <summary>
  /// Unit of work client side implementation
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class UnitOfWork<T> : IUnitOfWork<T> where T : ClientContext
  {
    private readonly T context;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="context"></param>
    public UnitOfWork(T context)
    {
      this.context = context;
    }

    /// <summary>
    /// Gets the database context
    /// </summary>
    public T Context => context;

    /// <summary>
    /// Commits changes
    /// </summary>
    /// <param name="bypassOperations">True to do not track changes into Operation DbSet. Default is False</param>
    /// <returns></returns>
    public async Task CommitAsync(bool bypassOperations)
    {
      using (var transaction = await BeginTransaction())
      {
        await Context.CommitAsync(bypassOperations);

        transaction.Commit();
      }
    }

    /// <summary>
    /// Start a new transaction
    /// </summary>
    /// <param name="cancellationToken">A cancellation token</param>
    /// <returns></returns>
    public async Task<IDbContextTransaction> BeginTransaction(CancellationToken cancellationToken = default(CancellationToken))
    {
      return await context.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Commits changes
    /// </summary>
    /// <returns></returns>
    public Task CommitAsync()
    {
      return CommitAsync(false);
    }
  }
}
