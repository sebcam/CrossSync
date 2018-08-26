//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;
//using CrossSync.Entity;
//using CrossSync.Entity.Abstractions;
//using CrossSync.Entity.Abstractions.Services;
//using CrossSync.AspNetCore.Exceptions;
//using Microsoft.EntityFrameworkCore;

//namespace CrossSync.AspNetCore.Services
//{
//  /// <summary>
//  /// Entity CRUD Service implementation
//  /// </summary>
//  /// <typeparam name="T"></typeparam>
//  public class SyncRepository<T> : ISyncRepository<T> where T : class, IVersionableEntity, new()
//  {
//    private readonly IContext context;

//    /// <summary>
//    /// Ctor
//    /// </summary>
//    /// <param name="context"></param>
//    public SyncRepository(IContext context)
//    {
//      this.context = context;
//      Set = context.Set<T>();
//    }

//    /// <summary>
//    /// Gets the corresponding DbSet
//    /// </summary>
//    public DbSet<T> Set { get; }

//    /// <summary>
//    /// Adds an entity
//    /// </summary>
//    /// <param name="entity"></param>
//    /// <returns></returns>
//    public virtual async Task<T> AddAsync(T entity)
//    {
//      await Set.AddAsync(entity);
//      return entity;
//    }

//    /// <summary>
//    /// Deletes an entity
//    /// </summary>
//    /// <param name="entity"></param>
//    /// <returns></returns>
//    public Task DeleteAsync(T entity)
//    {
//      Set.Remove(entity);
//      return Task.FromResult(0);
//    }

//    /// <summary>
//    /// Returns all entities
//    /// </summary>
//    /// <param name="predicate">Predicate used to filter entities</param>
//    /// <returns></returns>
//    public virtual Task<IQueryable<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null)
//    {
//      if (predicate != null)
//        return Task.FromResult(Set.Where(predicate));

//      return Task.FromResult(Set.AsQueryable());
//    }

//    /// <summary>
//    /// Returns the entity by ID
//    /// </summary>
//    /// <param name="id"></param>
//    /// <returns></returns>
//    public async Task<T> GetAsync(Guid id)
//    {
//      return await Set.FirstOrDefaultAsync(f => f.Id == id);
//    }

//    /// <summary>
//    /// Updates an entity
//    /// </summary>
//    /// <param name="id"></param>
//    /// <param name="value"></param>
//    /// <returns></returns>
//    public virtual async Task<T> UpdateAsync(Guid id, T value)
//    {
//      var existing = await GetAsync(id);
//      if (existing != null)
//      {
//        if (value.Version != null && existing.Version.SequenceEqual(value.Version))
//        {
//          context.Entry(existing).CurrentValues.SetValues(value);
//        }
//        else
//        {
//          throw new SyncConflictVersionException<T>(value, existing);
//        }
//        return existing;
//      }
//      else
//      {
//        return await AddAsync(value);
//      }
//    }
//  }
//}
