using System.Threading.Tasks;

namespace CrossSync.Entity.Abstractions.UnitOfWork
{
  /// <summary>
  /// Simple unit of work interface
  /// </summary>
  public interface IUnitOfWork
  {
    /// <summary>
    /// Commits changes
    /// </summary>
    /// <returns></returns>
    Task CommitAsync();
  }
}
