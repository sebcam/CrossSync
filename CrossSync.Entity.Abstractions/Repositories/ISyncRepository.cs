using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CrossSync.Entity.Abstractions.Services
{
  /// <summary>
  /// Simple CRUD service interface
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface ISyncRepository<T> where T : class, IIdentifiable, new()
  {
    /// <summary>
    /// Returns all entities
    /// </summary>
    /// <param name="predicate">Predicate used to filter entities</param>
    /// <returns></returns>
    Task<IQueryable<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null);

    /// <summary>
    /// Returns the entitie by Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<T> GetAsync(Guid id);

    /// <summary>
    /// Adds an entity
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Deletes an entity
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task DeleteAsync(T entity);

    /// <summary>
    /// Updates an entity
    /// </summary>
    /// <param name="id"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    Task<T> UpdateAsync(Guid id, T value);
  }
}
