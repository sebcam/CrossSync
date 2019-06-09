using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrossSync.Entity.Abstractions.Entities;
using CrossSync.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq;

namespace CrossSync.Infrastructure.Server
{
  internal class ServerOperationProxy : IServerOperationProxy
  {
    ServerContext context;

    public ServerOperationProxy(ServerContext context)
    {
      this.context = context;
    }

    public async Task TrackEntities(IEnumerable<EntityEntry> entries)
    {
      foreach (var entry in entries.ToList())
      {
        if (entry.Entity is IVersionableEntity versionableEntity)
        {
          if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
          {
            versionableEntity.UpdatedAt = DateTimeOffset.UtcNow;
          }
          else if (entry.State == EntityState.Deleted)
          {
            await context.DeletedEntities.AddAsync(new DeletedEntity
            {
              EntityId = versionableEntity.Id,
              DeletedDate = DateTimeOffset.UtcNow,
              DataType = entry.Entity.GetType().Name
            });
          }
        }
      }
    }
  }
}