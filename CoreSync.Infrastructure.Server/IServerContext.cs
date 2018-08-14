using CoreSync.Entity.Abstractions;
using CoreSync.Entity.Abstractions.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreSync.Infrastructure.Server
{
  /// <summary>
  /// Server context interface
  /// </summary>
  public interface IServerContext : IContext
  {
    /// <summary>
    /// DbSet where deleted entities informations are stored
    /// </summary>
    DbSet<DeletedEntity> DeletedEntities { get; }
  }
}