using System.Threading;
using System.Threading.Tasks;
using PropertyChanged;

namespace CrossSync.Xamarin.Mvvm.Models
{
  /// <summary>
  /// Simple view model implementation
  /// </summary>
  [AddINotifyPropertyChangedInterface]
  public abstract class BaseViewModel : IViewModel
  {
    private string title;
    private bool isBusy;

    /// <summary>
    /// Gets or sets whether the model is busy
    /// </summary>
    public bool IsBusy
    {
      get
      {
        return isBusy || IsInitializing;
      }
      set
      {
        isBusy = value;
      }
    }

    /// <summary>
    /// Gets the view model title
    /// </summary>
    public string Title
    {
      get => Modified ? $"{title} (Modifié)" : title;
      set => title = value;
    }

    /// <summary>
    /// Gets whether the viewmodel is modified
    /// </summary>
    [AlsoNotifyFor(nameof(Title))]
    public bool Modified { get; set; }

    /// <summary>
    /// Gets whether the view model is initializing
    /// </summary>
    [AlsoNotifyFor(nameof(IsBusy))]
    public bool IsInitializing { get; set; }

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual Task Initialize(CancellationToken cancellationToken)
    {
      return Task.FromResult(0);
    }

    Task IViewModel.Initialize(CancellationToken cancellationToken)
    {
      return Initialize(cancellationToken);
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public virtual void Dispose()
    {
    }
  }

  /// <summary>
  /// View model implementation using initialization parameter
  /// </summary>
  /// <typeparam name="TParam"></typeparam>
  public abstract class BaseViewModel<TParam> : BaseViewModel, IViewModel<TParam>
  {
    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public abstract Task Initialize(TParam param);
  }
}
