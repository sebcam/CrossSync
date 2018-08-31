using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CrossSync.Entity.Abstractions.EF.UnitOfWork;
using CrossSync.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage;

namespace CrossSync.Infrastructure.Server.UnitOfWork
{
  /// <summary>
  /// Entity framework unit of work implementation
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class UnitOfWork<T> : IUnitOfWork<T> where T : ServerContext
  {
    private readonly T context;
    private readonly IMediator mediator;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="context"></param>
    /// <param name="mediator"></param>
    public UnitOfWork(T context, IMediator mediator)
    {
      this.context = context;
      this.mediator = mediator;
    }

    /// <summary>
    /// Gets the database context
    /// </summary>
    public T Context => context;

    /// <summary>
    /// Starts an new transaction
    /// </summary>
    /// <param name="cancellationToken">A cancellation token</param>
    /// <returns></returns>
    public Task<IDbContextTransaction> BeginTransaction(CancellationToken cancellationToken = default(CancellationToken))
    {
      return context.Database.BeginTransactionAsync();
    }

    /// <summary>
    /// Commits changes
    /// </summary>
    /// <returns></returns>
    public async Task CommitAsync()
    {
      var ctx = context;      

      var events = ctx.ChangeTracker.Entries().Select(f => f.Entity).OfType<INotifiableEntity>().SelectMany(f => f.DomainEvents);
      if (events.Any())
        await Task.WhenAll(events.Select(async (d) => await mediator.Publish(d)).ToList());

      await context.CommitAsync();
    }
  }
}
