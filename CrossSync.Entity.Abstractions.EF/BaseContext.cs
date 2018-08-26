using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CrossSync.Entity.Abstractions
{
  /// <summary>
  /// 
  /// </summary>
  public abstract class BaseContext : DbContext, IContext
  {
    /// <summary>
    /// Gets the operation proxy
    /// </summary>
    protected abstract IOperationProxy Proxy { get; }

    /// <summary>
    /// Commits all changes into database
    /// </summary>
    /// <returns></returns>
    public async Task<int> CommitAsync()
    {
      var modifiedOwned = ChangeTracker.Entries().Where(f => f.Metadata.IsOwned() && f.State == EntityState.Modified).ToList();

      var modi = modifiedOwned.Select(f =>
      {
        var entries2 = ChangeTracker.Entries().Where(g => !g.Metadata.IsOwned());

        var refs = entries2.SelectMany(g => g.References).ToList();

        var e = refs.FirstOrDefault(g => g.TargetEntry.Entity == f.Entity).EntityEntry;

        return e;
      }).ToList();

      foreach (var modified in modi)
        modified.State = EntityState.Modified;

      await Proxy.TrackEntities(ChangeTracker.Entries().Where(f => !f.Metadata.IsOwned() && (f.State == EntityState.Added || f.State == EntityState.Deleted || f.State == EntityState.Modified)));

      return await base.SaveChangesAsync();
    }

    /// <summary>
    /// Context configuration with logging
    /// </summary>
    /// <param name="optionsBuilder"></param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      base.OnConfiguring(optionsBuilder);
      var lf = new LoggerFactory();
      lf.AddProvider(new ContextLogger());
      optionsBuilder.UseLoggerFactory(lf);
    }
  }
}
