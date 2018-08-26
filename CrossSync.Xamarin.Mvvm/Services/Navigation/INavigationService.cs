using System;
using System.Threading.Tasks;
using CrossSync.Xamarin.Mvvm.Models;
using Xamarin.Forms;

namespace CrossSync.Xamarin.Mvvm.Services.Navigation
{
  /// <summary>
  /// Navigation service implementation
  /// </summary>
  public interface INavigationService
  {
    /// <summary>
    /// Navigates to a new page
    /// </summary>
    /// <typeparam name="T">Type of viewmodel</typeparam>
    /// <returns></returns>
    Task NavigateAsync<T>() where T : class, IViewModel;

    /// <summary>
    /// Navigates to a new page
    /// </summary>
    /// <typeparam name="T">Type of viewmodel</typeparam>
    /// <param name="navigation">custom navigation</param>
    /// <returns></returns>
    Task NavigateAsync<T>(Func<Page, Task> navigation) where T : class, IViewModel;

    /// <summary>
    /// Navigates to a new page
    /// </summary>
    /// <typeparam name="T">Type of viewmodel</typeparam>
    /// <typeparam name="TParam">Type of parameter</typeparam>
    /// <param name="param">Parameter for view model initialization</param>
    /// <returns></returns>
    Task NavigateAsync<T, TParam>(TParam param) where T : class, IViewModel<TParam>;

    /// <summary>
    /// Displays view model into a dialog
    /// </summary>
    /// <typeparam name="T">Type of viewmodel</typeparam>
    /// <returns></returns>
    Task NavigateModalAsync<T>() where T : class, IViewModel;

    /// <summary>
    /// Display view model into a dialog and waiting for a result
    /// </summary>
    /// <typeparam name="T">Type of viewmodel</typeparam>
    /// <typeparam name="TResult">Type of result</typeparam>
    /// <param name="navigation">cusom navigation</param>
    Task<TResult> NavigateModalAsync<T, TResult>(Func<Page, Task> navigation = null) where T : class, IViewModel;

    /// <summary>
    /// Displays view model into a dialog and waiting for a result
    /// </summary>
    /// <typeparam name="T">Type of viewmodel</typeparam>
    /// <typeparam name="TParam">Type of parameter</typeparam>
    /// <typeparam name="TResult">Type of result</typeparam>
    /// <param name="navigation">cusom navigation</param>
    /// <param name="param">Parameter</param>
    /// <returns></returns>
    Task<TResult> NavigateModalAsync<T, TParam, TResult>(TParam param, Func<Page, Task> navigation = null) where T : class, IViewModel<TParam>;

    /// <summary>
    /// Closes a popup
    /// </summary>
    /// <typeparam name="TViewModel">Type of viewmodel</typeparam>
    /// <param name="viewModel">The current displayed popup view model</param>
    /// <returns></returns>
    Task PopAsync<TViewModel>(TViewModel viewModel) where TViewModel : IViewModel;
    
    /// <summary>
    /// Close the modal
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="viewModel"></param>
    /// <returns></returns>
    Task PopModalAsync<TViewModel>(TViewModel viewModel) where TViewModel : IViewModel;

    /// <summary>
    /// Closes the modal
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="viewModel"></param>
    /// <param name="result"></param>
    /// <param name="navigation"></param>
    /// <returns></returns>
    Task PopModalAsync<T, TResult>(T viewModel, TResult result, Func<Task> navigation = null) where T : class, IViewModel;

    /// <summary>
    /// Close all views and back to root
    /// </summary>
    /// <returns></returns>
    Task PopToRootAsync();

    /// <summary>
    /// The native navigation service
    /// </summary>
    INavigation Navigation { get; }
  }
}
