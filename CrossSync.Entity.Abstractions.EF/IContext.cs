using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CrossSync.Entity.Abstractions
{
  /// <summary>
  /// Base database context interface
  /// </summary>
  public interface IContext
  {
    /// <summary>
    /// Returns the entity DbSet of type
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <returns></returns>
    DbSet<T> Set<T>() where T : class;

    /// <summary>
    /// Returns the entity entry
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="entity">The entity</param>
    /// <returns></returns>
    EntityEntry<T> Entry<T>(T entity) where T : class;
  }
}