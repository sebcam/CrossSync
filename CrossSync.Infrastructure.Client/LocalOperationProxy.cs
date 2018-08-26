using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq;
using System;
using CrossSync.Entity.Abstractions.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using CrossSync.Entity;

namespace CrossSync.Infrastructure.Client
{
  /// <summary>
  /// Local proxy operation implementation
  /// </summary>
  public class LocalOperationProxy : ILocalOperationProxy
  {
    ClientContext context;

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="context"></param>
    public LocalOperationProxy(ClientContext context)
    {
      this.context = context;
    }

    /// <summary>
    /// Track the changed entities 
    /// The changed entities are stored into the Operations DbSet
    /// </summary>
    /// <param name="entries">Modified or deleted entries</param>
    /// <returns></returns>
    public async Task TrackEntities(IEnumerable<EntityEntry> entries)
    {
      var entryIds = entries.Where(f => !f.Metadata.IsOwned() && f.Entity is IVersionableEntity).Select(f => f.GetEntityId<Guid>()).ToList();
      var existingOperations = context.Operations.Where(f => entryIds.Contains(f.EntityId)).ToList();

      foreach (var entry in entries.Where(f => !f.Metadata.IsOwned() && f.Entity is IVersionableEntity).ToList())
      {
        var entityId = entry.GetEntityId<Guid>();
        if (entityId == Guid.Empty)
          continue;

        var existing = existingOperations.FirstOrDefault(f => f.EntityId == entityId);
        if (entry.State == EntityState.Deleted && existing != null && string.IsNullOrEmpty(existing.Version))
        {
          context.Remove(existing);
          continue;
        }

        var operation = new Operation
        {
          Id = existing?.Id ?? Guid.NewGuid(),
          EntityId = entityId,
          Status = existing != null && existing.Version == null ? EntityState.Added : entry.State,
          UpdatedAt = DateTimeOffset.UtcNow,
          Version = entry.GetEntityVersion(),
          DataType = entry.Entity.GetType().Name
        };

        Debug.WriteLine(operation);

        if (existing == null)
          await context.Operations.AddAsync(operation);
        else
          context.Entry<Operation>(existing).CurrentValues.SetValues(operation);
      }
    }
  }
}