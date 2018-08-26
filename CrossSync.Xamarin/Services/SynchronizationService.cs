using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CrossSync.Xamarin.Services
{
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
    }

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
