using System.Threading.Tasks;
using CrossSync.Entity.Abstractions;

namespace CrossSync.Xamarin.Services
{
  /// <summary>
  /// Interface that allow to handle a synchronization conflict
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface IConflictHandler<T> where T : class, IIdentifiable
  {
    /// <summary>
    /// Handles the conflict
    /// </summary>
    /// <param name="clientValue">Client side entity</param>
    /// <param name="serverValue">Server side entity</param>
    /// <returns>The winner entity</returns>
    Task<T> HandleConflict(T clientValue, T serverValue);
  }
}