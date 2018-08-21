using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using CoreSync.Entity;
using CoreSync.Entity.Abstractions;
using CoreSync.Entity.Abstractions.Entities;
using CoreSync.Infrastructure.Client;
using CoreSync.Xamarin.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace CoreSync.Xamarin.Services
{
  public class SyncContext<T> where T : class, IVersionableEntity, new()
  {
    private readonly IClientContext context;
    private readonly IMobileSyncService<T> service;
    private readonly Lazy<IErrorService> errorService;
    private DateTimeOffset lastSync;
    private readonly string baseUrl;
    private readonly string deletedRelativeUri;

    public SyncContext(IClientContext context, IMobileSyncService<T> service, Lazy<IErrorService> errorService)
    {
      this.context = context;
      this.service = service;
      this.errorService = errorService;
      lastSync = DateTimeOffset.MinValue;
      baseUrl = IoC.ResolveNamed<string>("ApiBaseUrl");
      deletedRelativeUri = IoC.ResolveNamed<string>("DeletedRelativeUri");
    }

    public async Task SyncAsync()
    {
      Debug.WriteLine($"Synchro de : {typeof(T).Name}");

      var connectivity = IoC.Resolve<IConnectivityService>();

      if (!Debugger.IsAttached && !connectivity.IsConnected)
      {
        return;
      }
      if (!Debugger.IsAttached && !await connectivity.IsRemoteReachable(baseUrl))
      {
        errorService.Value.ShowError($"Server not available ({baseUrl})");
        return;
      }

      if (Application.Current.Properties.TryGetValue($"{typeof(T).Name}.LastSynchroDate", out object date))
      {
        Debug.WriteLine($"Last sync done : {date}");
        lastSync = DateTimeOffset.Parse(date.ToString());
      }
      else
        lastSync = DateTimeOffset.MinValue;

      var nowSyncDate = DateTimeOffset.Now;

      await PushAsync();

      await PullAsync();

      Application.Current.Properties[$"{typeof(T).Name}.LastSynchroDate"] = nowSyncDate;

      await Application.Current.SavePropertiesAsync();
    }

    public async Task DeleteAsync()
    {
      Debug.WriteLine("DeleteAsync");
      try
      {
        using (var client = new HttpClient
        {
          BaseAddress = new Uri(baseUrl),
          Timeout = new TimeSpan(0, 0, 5)
        })
        {
          var response = await client.GetAsync(deletedRelativeUri.TrimEnd('/') + "/" + typeof(T).Name);
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
        BaseAddress = new Uri(baseUrl),
        Timeout = new TimeSpan(0, 0, 5)
      })
      {
        var operations = context.Operations.Where(f => f.DataType == typeof(T).Name).OrderBy(f => f.UpdatedAt).ToList();
        var ids = operations.Where(f => f.Status != EntityState.Deleted).Select(f => f.EntityId).ToList();
        IEnumerable<T> items = (await service.GetAllAsync(f => ids.Contains(f.Id))).ToList();

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
                  response = await client.DeleteAsync($"{service.ApiUri}/{operation.EntityId}");
                  response.EnsureSuccessStatusCode();
                  break;
                case EntityState.Modified:
                  response = await client.PutAsync($"{service.ApiUri}/{operation.EntityId}", new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json"));
                  if (!response.IsSuccessStatusCode)
                  {
                    if (response.StatusCode == HttpStatusCode.Conflict)
                    {
                      var serverValue = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());

                      if (service is IConflictHandler<T> conflictHandler)
                      {
                        freshEntity = (await conflictHandler.HandleConflict(item, serverValue));
                        if (freshEntity == item)
                        {
                          freshEntity.Version = serverValue.Version;
                          response = await client.PutAsync($"{service.ApiUri}/{operation.EntityId}", new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json"));
                          response.EnsureSuccessStatusCode();
                        }
                      }
                      else
                        freshEntity = serverValue;
                    }
                  }
                  else
                    freshEntity = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
                  break;
                case EntityState.Added:
                  response = await client.PostAsync(service.ApiUri, new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json"));
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
              await service.CompleteUpdateAsync(item, freshEntity);
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
          BaseAddress = new Uri(baseUrl),
          Timeout = new TimeSpan(0, 0, 5)
        })
        {
          var response = await client.GetAsync(service.ApiUri + "?from=" + WebUtility.UrlEncode(lastSync.ToString()));
          response.EnsureSuccessStatusCode();

          var items = JsonConvert.DeserializeObject<IEnumerable<T>>(await response.Content.ReadAsStringAsync());
          var ids = items.Select(f => new { f.Id, f.Version });
          var entities = await service.GetAllAsync(f => ids.Any(g => g.Id == f.Id));
          var idsToUpdate = entities.Select(f => f.Id).ToList();
          var pendingOperations = context.Operations.Where(f => idsToUpdate.Contains(f.EntityId));

          Debug.WriteLine($"{items.Count()} éléments a récupérer");

          foreach (var entity in entities)
          {
            var serverValue = items.FirstOrDefault(f => f.Id == entity.Id);
            if (serverValue.Version == entity.Version)
              continue;

            var pendingOperation = pendingOperations.FirstOrDefault(f => f.EntityId == entity.Id);
            if (pendingOperation != null)
            {
              T resolvedEntity = serverValue;
              if (service is IConflictHandler<T> conflictHandler)
              {
                resolvedEntity = (await conflictHandler.HandleConflict(entity, serverValue));
                context.Operations.Remove(pendingOperation);
              }

              await service.UpdateAsync(entity.Id, resolvedEntity);
              context.Entry(entity).CurrentValues.SetValues(resolvedEntity);

              await service.CompleteUpdateAsync(entity, resolvedEntity);
              await service.UpdateForeignKeys(new[] { entity });
            }
            else
            {
              var newValuesEntity = items.First(f => f.Id == entity.Id);
              context.Entry(entity).CurrentValues.SetValues(newValuesEntity);

              await service.CompleteUpdateAsync(entity, newValuesEntity);
              await service.UpdateForeignKeys(new[] { entity });
            }
          }

          var itemsToAdd = items.Except(entities, new IdComparer());
          if (itemsToAdd.Any())
          {
            await service.UpdateForeignKeys(itemsToAdd);
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

  /// <summary>
  /// Global Synchronization implementation
  /// </summary>
  public class SynchronizationService
  {
    private readonly IEnumerable<ISyncService> services;

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="services">All synchronization service</param>
    public SynchronizationService(IEnumerable<ISyncService> services)
    {
      this.services = services;

      Debug.WriteLine($"Synchronisation de {services.Count()} services !");
    }

    /// <summary>
    /// Api URI
    /// </summary>
    public string ApiUri { get; set; }

    /// <summary>
    /// Sync
    /// </summary>
    /// <returns></returns>
    public async Task SyncAsync()
    {
      foreach (var service in services.OrderByDescending(f => f.Order))
      {
        await service.SyncDeletedAsync();
      }

      foreach (var service in services.OrderBy(f => f.Order))
      {
        await service.SyncAsync();
      }
    }
  }
}
