using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSync.Xamarin.Mvvm.Models
{
  /// <summary>
  /// View model interface
  /// </summary>
  public interface IViewModel : IDisposable
  {
    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task Initialize(CancellationToken cancellationToken = default(CancellationToken));

    /// <summary>
    /// Title
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Gets whether view model is initializing
    /// </summary>
    bool IsInitializing { get; set; }

    /// <summary>
    /// Gets whether view model is busy
    /// </summary>
    bool IsBusy { get; }
  }

  /// <summary>
  /// View model implementation using initialization parameter
  /// </summary>
  /// <typeparam name="TParam"></typeparam>
  public interface IViewModel<TParam> : IViewModel
  {
    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    Task Initialize(TParam param);
  }
}
