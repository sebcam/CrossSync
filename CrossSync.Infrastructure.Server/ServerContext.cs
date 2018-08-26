using CrossSync.Entity.Abstractions;
using CrossSync.Entity.Abstractions.Entities;
using Microsoft.EntityFrameworkCore;

namespace CrossSync.Infrastructure.Server
{
  /// <summary>
  /// Base DbContext 
  /// </summary>
  public abstract class ServerContext : BaseContext, IServerContext
  {
    IServerOperationProxy serverProxy;

    /// <summary>
    /// Gets the deleted entities
    /// </summary>
    public DbSet<DeletedEntity> DeletedEntities { get; set; }

    /// <summary>
    /// Gets the operation proxy
    /// </summary>
    protected virtual IServerOperationProxy ServerProxy => serverProxy ?? (serverProxy = new ServerOperationProxy(this));

    /// <summary>
    /// Gets the operation proxy
    /// </summary>
    protected sealed override IOperationProxy Proxy => ServerProxy;

    /// <summary>
    /// Database model creation
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<DeletedEntity>().HasKey(f => f.Id);
      modelBuilder.Entity<DeletedEntity>().HasIndex(nameof(DeletedEntity.EntityId), nameof(DeletedEntity.DataType)).IsUnique();
    }
  }
}
