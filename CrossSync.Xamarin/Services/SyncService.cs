using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CrossSync.Entity;
using CrossSync.Entity.Abstractions;
using CrossSync.Entity.Abstractions.EF.UnitOfWork;
using CrossSync.Entity.Abstractions.Entities;
using CrossSync.Entity.Abstractions.UnitOfWork;
using CrossSync.Infrastructure.Client;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace CrossSync.Xamarin.Services
{
  /// <summary>
  /// Base synchronization service implementation
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public abstract class SyncService<T> : IMobileSyncService<T>, ISyncService where T : class, IVersionableEntity, new()
  {
    private readonly IUnitOfWork uof;
    private readonly IConnectivityService connectivityService;
    private readonly Lazy<IErrorService> errorService;
    private readonly SyncConfiguration configuration;
    protected readonly IClientContext context;
    private DateTimeOffset lastSync = DateTimeOffset.MinValue;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="uof"></param>
    public SyncService(IUnitOfWork<IClientContext> uof, IConnectivityService connectivityService, Lazy<IErrorService> errorService, SyncConfiguration configuration)
    {
      this.uof = uof;
      this.connectivityService = connectivityService;
      this.errorService = errorService;
      this.configuration = configuration;
      this.context = uof.Context;
      Set = context.Set<T>();

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
      {
        return Task.FromResult(Includes(Set).Where(predicate));
      }

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
    public async Task SyncAsync()
    {
      Debug.WriteLine($"Synchro de : {typeof(T).Name}");

      if (!Debugger.IsAttached && !connectivityService.IsConnected)
      {
        return;
      }
      if (!Debugger.IsAttached && !await connectivityService.IsRemoteReachable(configuration.ApiBaseUrl))
      {
        errorService.Value.ShowError($"Server not available ({configuration.ApiBaseUrl})");
        return;
      }

      if (Application.Current.Properties.TryGetValue($"{typeof(T).Name}.LastSynchroDate", out object date))
      {
        Debug.WriteLine($"Last sync done : {date}");
        lastSync = DateTimeOffset.Parse(date.ToString());
      }
      else
      {
        lastSync = DateTimeOffset.MinValue;
      }

      var nowSyncDate = DateTimeOffset.Now;

      await PushAsync();

      await PullAsync();

      Application.Current.Properties[$"{typeof(T).Name}.LastSynchroDate"] = nowSyncDate;

      await Application.Current.SavePropertiesAsync();
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
    public async Task SyncDeletedAsync()
    {
      Debug.WriteLine("DeleteAsync");
      try
      {
        using (var client = new HttpClient
        {
          BaseAddress = new Uri(configuration.ApiBaseUrl),
          Timeout = new TimeSpan(0, 0, 5)
        })
        {
          var response = await client.GetAsync(new Uri(new Uri(configuration.TombstoneUri, UriKind.Relative), typeof(T).Name));
          response.EnsureSuccessStatusCode();

          var deletedRecords = JsonConvert.DeserializeObject<IEnumerable<DeletedEntity>>(await response.Content.ReadAsStringAsync());
          var deletedIds = deletedRecords.Select(f => f.EntityId).AsEnumerable();

          Debug.WriteLine($"{deletedIds.Count()} éléments a supprimer");

          var recordsToDelete = context.Set<T>().Where(f => deletedIds.Contains(f.Id)).AsEnumerable();
          if (recordsToDelete.Any())
          {
            context.Set<T>().RemoveRange(recordsToDelete);
            await context.CommitAsync();
          }
          var operationsToDelete = context.Operations.Where(f => deletedIds.Contains(f.EntityId)).AsEnumerable();
          if (operationsToDelete.Any())
          {
            context.Operations.RemoveRange(operationsToDelete);
            await context.CommitAsync();
          }
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"Exception en suppression : {ex.Message}");
        Debug.WriteLine($"Exception en suppression inner : {ex.InnerException}");
        Debug.WriteLine($"{ex.StackTrace}");
      }
    }

    private async Task PushAsync()
    {
      Debug.WriteLine("PushAsync");
      using (var client = new HttpClient
      {
        BaseAddress = new Uri(configuration.ApiBaseUrl),
        Timeout = new TimeSpan(0, 0, 5)
      })
      {
        var operations = context.Operations.Where(f => f.DataType == typeof(T).Name).OrderBy(f => f.UpdatedAt).ToList();
        var ids = operations.Where(f => f.Status != EntityState.Deleted).Select(f => f.EntityId).ToList();
        IEnumerable<T> items = (await GetAllAsync(f => ids.Contains(f.Id))).ToList();

        Debug.WriteLine($"{operations.Count} opérations a envoyer");

        foreach (var operation in operations)
        {
          Debug.WriteLine($"{operations.IndexOf(operation) + 1} : Commencée");
          HttpResponseMessage response = null;
          var item = operation.Status != EntityState.Deleted ? items.FirstOrDefault(f => f.Id == operation.EntityId) : null;
          T freshEntity = null;
          if (operation.Status == EntityState.Deleted || item != null)
          {
            try
            {
              switch (operation.Status)
              {
                case EntityState.Deleted:
                  response = await client.DeleteAsync($"{ApiUri}/{operation.EntityId}");
                  response.EnsureSuccessStatusCode();
                  break;
                case EntityState.Modified:
                  response = await client.PutAsync($"{ApiUri}/{operation.EntityId}", new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json"));
                  if (!response.IsSuccessStatusCode)
                  {
                    if (response.StatusCode == HttpStatusCode.Conflict)
                    {
                      var serverValue = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());

                      if (this is IConflictHandler<T> conflictHandler)
                      {
                        freshEntity = (await conflictHandler.HandleConflict(item, serverValue));
                        if (freshEntity == item)
                        {
                          freshEntity.Version = serverValue.Version;
                          response = await client.PutAsync($"{ApiUri}/{operation.EntityId}", new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json"));
                          response.EnsureSuccessStatusCode();
                        }
                      }
                      else
                      {
                        freshEntity = serverValue;
                      }
                    }
                  }
                  else
                  {
                    freshEntity = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
                  }

                  break;
                case EntityState.Added:
                  response = await client.PostAsync(ApiUri, new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json"));
                  response.EnsureSuccessStatusCode();
                  freshEntity = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
                  break;
                default:
                  break;
              }
            }
            catch (Exception e)
            {
              Debug.WriteLine(e.Message);
              continue;
            }
          }

          context.Operations.Remove(operation);
          if (operation.Status == EntityState.Added || operation.Status == EntityState.Modified)
          {
            if (response != null)
            {
              context.Entry(item).CurrentValues.SetValues(freshEntity);
              await CompleteUpdateAsync(item, freshEntity);
            }
          }
          await context.CommitAsync(true);

          Debug.WriteLine($"{operations.IndexOf(operation) + 1} : Terminée");
        }
      }
    }

    private async Task PullAsync()
    {
      Debug.WriteLine("PullAsync");
      try
      {
        using (var client = new HttpClient
        {
          BaseAddress = new Uri(configuration.ApiBaseUrl),
          Timeout = new TimeSpan(0, 0, 5)
        })
        {
          var response = await client.GetAsync(ApiUri + "?from=" + WebUtility.UrlEncode(lastSync.ToString()));
          response.EnsureSuccessStatusCode();

          var items = JsonConvert.DeserializeObject<IEnumerable<T>>(await response.Content.ReadAsStringAsync());
          var ids = items.Select(f => new { f.Id, f.Version });
          var entities = await GetAllAsync(f => ids.Any(g => g.Id == f.Id));
          var idsToUpdate = entities.Select(f => f.Id).ToList();
          var pendingOperations = context.Operations.Where(f => idsToUpdate.Contains(f.EntityId));

          Debug.WriteLine($"{items.Count()} éléments a récupérer");

          foreach (var entity in entities)
          {
            var serverValue = items.FirstOrDefault(f => f.Id == entity.Id);
            if (serverValue.Version == entity.Version)
            {
              continue;
            }

            var pendingOperation = pendingOperations.FirstOrDefault(f => f.EntityId == entity.Id);
            if (pendingOperation != null)
            {
              T resolvedEntity = serverValue;
              if (this is IConflictHandler<T> conflictHandler)
              {
                resolvedEntity = (await conflictHandler.HandleConflict(entity, serverValue));
                context.Operations.Remove(pendingOperation);
              }

              await UpdateAsync(entity.Id, resolvedEntity);
              context.Entry(entity).CurrentValues.SetValues(resolvedEntity);

              await CompleteUpdateAsync(entity, resolvedEntity);
              await UpdateForeignKeys(new[] { entity });
            }
            else
            {
              var newValuesEntity = items.First(f => f.Id == entity.Id);
              context.Entry(entity).CurrentValues.SetValues(newValuesEntity);

              await CompleteUpdateAsync(entity, newValuesEntity);
              await UpdateForeignKeys(new[] { entity });
            }
          }

          var itemsToAdd = items.Except(entities, new IdComparer());
          if (itemsToAdd.Any())
          {
            await UpdateForeignKeys(itemsToAdd);
            await (context as DbContext).AddRangeAsync(itemsToAdd);
          }

          await context.CommitAsync(true);
        }
      }
      catch (HttpRequestException ex)
      {
        Debug.WriteLine(ex);
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"Exception en synchro : {ex.Message}");
        Debug.WriteLine($"Exception en synchro inner : {ex.InnerException}");
        Debug.WriteLine($"{ex.StackTrace}");
      }
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

  class IdComparer : IEqualityComparer<IIdentifiable>
  {
    public bool Equals(IIdentifiable x, IIdentifiable y)
    {
      return x.Id == y.Id;
    }

    public int GetHashCode(IIdentifiable obj)
    {
      return obj != null ? obj.Id.GetHashCode() : 0;
    }
  }
}
