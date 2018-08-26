using CrossSync.Entity.Abstractions;
using CrossSync.Entity.Abstractions.Entities;
using Microsoft.EntityFrameworkCore;

namespace CrossSync.Infrastructure.Server
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