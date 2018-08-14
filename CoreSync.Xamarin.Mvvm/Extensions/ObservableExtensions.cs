using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CoreSync.Xamarin.Mvvm.Extensions
{
  /// <summary>
  /// Observable extensions
  /// </summary>
  public static class ObservableExtension
  {
    /// <summary>
    /// Transforms an enumerable list to an observable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
    {
      return new ObservableCollection<T>(source);
    }
  }
}
