using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CrossSync.Entity.Abstractions;

namespace CrossSync.Xamarin.Services
{
  /// <summary>
  /// Sync CRUD service implementation
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface IMobileSyncService<T> : ISyncService where T : class, IIdentifiable, new()
  {
    /// <summary>
    /// The api URI
    /// </summary>
    string ApiUri { get; }

    /// <summary>
    /// Adds an entity
    /// </summary>
    /// <param name="entity"></param>
    /// <returns>The added entity</returns>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Deletes an entity
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task DeleteAsync(T entity);

    /// <summary>
    /// Queries all entities
    /// </summary>
    /// <param name="predicate">Predicate to filter the query</param>
    /// <returns></returns>
    Task<IQueryable<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null);

    /// <summary>
    /// Returns the entity by ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<T> GetAsync(Guid id);

    /// <summary>
    /// Updates the entity
    /// </summary>
    /// <param name="id"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    Task<T> UpdateAsync(Guid id, T value);

    /// <summary>
    /// Completes the update
    /// </summary>
    /// <param name="entity">Entity to update</param>
    /// <param name="values">New value entity</param>
    /// <returns></returns>
    Task CompleteUpdateAsync(T entity, T values);

    /// <summary>
    /// Updates the foreign keys
    /// </summary>
    /// <param name="itemsToAdd"></param>
    /// <returns></returns>
    Task UpdateForeignKeys(IEnumerable<IIdentifiable> itemsToAdd);
  }

  /// <summary>
  /// Synchronization service interface
  /// </summary>
  public interface ISyncService
  {

    /// <summary>
    /// Order used to order services synchronization
    /// </summary>
    //TODO : Voir pour le générer automatiquement en fonction des relations
    int Order { get; }

    /// <summary>
    /// Synchronizes added and modified entities and get latest. (Push & Pull)
    /// </summary>
    /// <returns></returns>
    Task SyncAsync();

    /// <summary>
    /// Synchronizes the deleted entities
    /// </summary>
    /// <returns></returns>
    Task SyncDeletedAsync();
  }
}