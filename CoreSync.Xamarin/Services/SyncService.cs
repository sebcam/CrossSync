using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CoreSync.Entity;
using CoreSync.Entity.Abstractions;
using CoreSync.Entity.Abstractions.EF.UnitOfWork;
using CoreSync.Entity.Abstractions.UnitOfWork;
using CoreSync.Infrastructure.Client;
using CoreSync.Xamarin.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace CoreSync.Xamarin.Services
{
  /// <summary>
  /// Base synchronization service implementation
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public abstract class SyncService<T> : IMobileSyncService<T>, ISyncService where T : class, IVersionableEntity, new()
  {
    private readonly IUnitOfWork uof;
    private readonly Lazy<IErrorService> errorService;
    protected readonly IClientContext context;
    private readonly SyncContext<T> syncContext;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="uof"></param>
    public SyncService(IUnitOfWork<IClientContext> uof, Lazy<IErrorService> errorService)
    {
      this.uof = uof;
      this.errorService = errorService;
      this.context = uof.Context;
      Set = context.Set<T>();

      syncContext = new SyncContext<T>(context, this, errorService);
    }

    /// <summary>
    /// Order
    /// </summary>
    public abstract int Order { get; }

    /// <summary>
    /// Gets the Entity DbSet
    /// </summary>
    public DbSet<T> Set { get; }

    /// <summary>
    /// Gets the Api URI (eg. "api/customer")
    /// </summary>
    public abstract string ApiUri { get; }

    /// <summary>
    /// Adds an entity
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task<T> AddAsync(T entity)
    {
      await Set.AddAsync(entity);
      await context.CommitAsync();
      return entity;
    }

    /// <summary>
    /// Deletes an entity
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task DeleteAsync(T entity)
    {
      Set.Remove(entity);
      await context.CommitAsync();
    }

    /// <summary>
    /// Queries all entities
    /// </summary>
    /// <param name="predicate">Predicate to filter the query</param>
    /// <returns></returns>
    public virtual Task<IQueryable<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null)
    {
      if (predicate != null)
        return Task.FromResult(Includes(Set).Where(predicate));

      return Task.FromResult(Includes(Set));
    }

    /// <summary>
    /// Returns the entity by ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public virtual async Task<T> GetAsync(Guid id)
    {
      return await Includes(Set).FirstOrDefaultAsync(f => f.Id == id);
    }

    /// <summary>
    /// Sync
    /// </summary>
    /// <returns></returns>
    public Task SyncAsync()
    {
      return syncContext.SyncAsync();
    }

    /// <summary>
    /// Includes
    /// </summary>
    protected virtual Func<IQueryable<T>, IQueryable<T>> Includes { get; } = f => f;

    /// <summary>
    /// Updates the entity
    /// </summary>
    /// <param name="id"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public async Task<T> UpdateAsync(Guid id, T value)
    {
      var existing = await GetAsync(id);
      if (existing != null)
      {
        if (existing.Version == value.Version)
        {
          context.Entry(existing).CurrentValues.SetValues(value);

          await CompleteUpdateAsync(existing, value);

          await context.CommitAsync();
        }
        else
        {
          errorService.Value.ShowError("L'enregistrement n'a pas été enregistré car il n'était pas a jour.");
          //throw new SyncConflictVersionException<T>(value, existing);
        }
        return existing;
      }
      else
      {
        return await AddAsync(value);
      }
    }

    /// <summary>
    /// Completes the update
    /// </summary>
    /// <param name="entity">Entity to update</param>
    /// <param name="values">New value entity</param>
    /// <returns></returns>
    public virtual Task CompleteUpdateAsync(T existing, T value)
    {
      return Task.FromResult(0);
    }

    /// <summary>
    /// Sync deleted
    /// </summary>
    /// <returns></returns>
    public Task SyncDeletedAsync()
    {
      return syncContext.DeleteAsync();
    }

    /// <summary>
    /// Updates the foreign keys
    /// </summary>
    /// <param name="itemsToAdd"></param>
    /// <returns></returns>
    public virtual Task UpdateForeignKeys(IEnumerable<IIdentifiable> itemsToAdd)
    {
      return Task.FromResult(0);
    }
  }
}
