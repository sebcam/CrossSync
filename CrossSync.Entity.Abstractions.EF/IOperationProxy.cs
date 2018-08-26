using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CrossSync.Entity.Abstractions
{
  /// <summary>
  /// Operation proxy interface used to track entities changes
  /// </summary>
  public interface IOperationProxy
  {
    /// <summary>
    /// Track the entities changes
    /// </summary>
    /// <param name="entries">List of entries which has been modified</param>
    /// <returns></returns>
    Task TrackEntities(IEnumerable<EntityEntry> entries);
  }
}
