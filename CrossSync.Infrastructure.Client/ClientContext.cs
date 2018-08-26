using System.Threading.Tasks;
using CrossSync.Entity.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CrossSync.Infrastructure.Client
{
  /// <summary>
  /// Client entity framework context
  /// </summary>
  public abstract class ClientContext : BaseContext, IClientContext
  {
    private ILocalOperationProxy localProxy;

    /// <summary>
    /// Gets the operation proxy used to track changes
    /// </summary>
    protected virtual ILocalOperationProxy LocalProxy => localProxy ?? (localProxy = new LocalOperationProxy(this));

    /// <summary>
    /// Gets the operation proxy used to track changes
    /// </summary>
    protected sealed override IOperationProxy Proxy => LocalProxy;

    /// <summary>
    /// Gets the operation DbSet where are stored database changes used to synchronize client database with server
    /// </summary>
    public DbSet<Operation> Operations { get; set; }

    /// <summary>
    /// Commits the entity changes
    /// </summary>
    /// <param name="bypassOperations">True to do not track changes into Operation DbSet. Default is False</param>
    public Task<int> CommitAsync(bool bypassOperations = false)
    {
      if (bypassOperations)
      {
        return SaveChangesAsync();
      }
      return base.CommitAsync();
    }
  }
}
