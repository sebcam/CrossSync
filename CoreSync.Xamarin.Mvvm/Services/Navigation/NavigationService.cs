using System;
using System.Threading.Tasks;
using Autofac;
using Xamarin.Forms;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using CoreSync.Xamarin.Mvvm.Models;
using CoreSync.Xamarin.Dependency;
using System.Threading;

namespace CoreSync.Xamarin.Mvvm.Services.Navigation
{
  public class NavigationService : INavigationService
  {
    private readonly ConditionalWeakTable<IViewModel, TaskCompletionSource<object>> results = new ConditionalWeakTable<IViewModel, TaskCompletionSource<object>>();

    private readonly ConditionalWeakTable<Page, ILifetimeScope> pageScopes = new ConditionalWeakTable<Page, ILifetimeScope>();

    public Task NavigateAsync<T>() where T : class, IViewModel
    {
      return this.InternalNavigateAsync<T>();
    }
    public Task NavigateAsync<T>(Func<Page, Task> navigation) where T : class, IViewModel
    {
      return this.InternalNavigateAsync<T>(navigation);
    }

    private async Task InternalNavigateAsync<T>(Func<Page, Task> navigation = null, Func<T, Task> init = null) where T : class, IViewModel
    {
      if (navigation == null)
      {
        navigation = async (p) => await Navigation.PushAsync(p);
      }
      try
      {
        //Device.BeginInvokeOnMainThread(() =>);
        var page = await ResolvePage<T>();
        if (page == null)
        {
          await PopToRootAsync();
          return;
        }

        CancellationTokenSource cts = new CancellationTokenSource();

        // Initialisation dans le Appearing pour recharger automatiquement les pages a chaque fois qu'elles réapparaissent.
        page.Appearing += async (s, e) =>
        {
          try
          {
            var scope = IoC.Container.BeginLifetimeScope();
            pageScopes.Add(page, scope);

            var viewModel = scope.Resolve<T>();
            if (viewModel == null)
            {
              await PopToRootAsync();
              return;
            }
            try
            {
              Debug.WriteLine($"Appearing : {viewModel.GetType().Name}");
              viewModel.IsInitializing = true;

              page.SetBinding(Page.TitleProperty, nameof(viewModel.Title));

              page.BindingContext = viewModel;

              if (!cts.IsCancellationRequested)
              {
                Debug.WriteLine($"Initializing view model : {viewModel.GetType().Name}");
                await viewModel.Initialize(cts.Token);
                if (init != null)
                  await init(viewModel);
              }
              else
                Debug.WriteLine($"Appearing CANCELLED : {viewModel.GetType().Name}");
            }
            finally
            {
              cts = new CancellationTokenSource();
              viewModel.IsInitializing = false;
            }
          }
          finally
          {
            //cts.Dispose();
            //Device.BeginInvokeOnMainThread(() => dialogService.HideLoading());
          }
        };

        // Dispose le scope incluant les services, view models, et SURTOUT le DbContext et UnitOfWork
        // Cela permet notamment de garder un object DbContext par page et non global a l'appli pour ne pas qu'il prenne trop de mémoire.
        // Ces objects seront réinjectés lors du appearing si la page est réaffichée
        page.Disappearing += async (s, e) =>
        {
          Debug.WriteLine($"CanBeCanceled : {cts.Token.CanBeCanceled}");

          if (cts.Token.CanBeCanceled && page.BindingContext == null)
            cts.Cancel();

          if (pageScopes.TryGetValue(page, out ILifetimeScope pageScope))
          {
            if (page.BindingContext is T viewModel)
            {
              while (viewModel.IsBusy)
              {
                await Task.Delay(200);
                Debug.WriteLine("ViewModel is busy while disposing... Waiting for busy to be false.");
              };
            }
            Debug.WriteLine("Scope disposed !");
            pageScope.Dispose();
            pageScopes.Remove(page);
          }
        };

        await navigation(page);
      }
      catch (Exception e)
      {
        Debug.WriteLine(e.Message);

        throw;
      }
    }

    public Task NavigateAsync<T, TParam>(TParam param) where T : class, IViewModel<TParam>
    {
      return this.InternalNavigateAsync<T>(init: async (viewModel) =>
      {
        await viewModel.Initialize(param);
      });
    }

    public Task PopToRootAsync()
    {
      return Navigation.PopToRootAsync();
    }

    public Task PopAsync<TViewModel>(TViewModel viewModel) where TViewModel : IViewModel
    {
      try
      {
        return Navigation.PopAsync();
      }
      finally
      {
        viewModel?.Dispose();
      }
    }

    public INavigation Navigation => Application.Current.MainPage is MasterDetailPage masterDetail ? masterDetail.Detail.Navigation : Application.Current.MainPage.Navigation;

    public Task<Page> ResolvePage<T>(ILifetimeScope scope = null) where T : class, IViewModel
    {
      return ResolvePage(scope, typeof(T));
    }

    public Task<Page> ResolvePage(ILifetimeScope scope, Type viewModelType)
    {
      if (viewModelType == null)
        return null;

      var pageName = viewModelType.Name.Replace("ViewModel", "Page");
      var page = (scope ?? IoC.Container).ResolveNamed<Page>(pageName);

      return Task.FromResult(page);
    }

    //public async Task<Page> ResolveAndInitPage(Type viewModelType)
    //{
    //  var viewModel = IoC.Resolve<IViewModel>(viewModelType);
    //  if (viewModel == null)
    //  {
    //    await PopToRootAsync();
    //  }
    //  try
    //  {
    //    viewModel.IsInitializing = true;

    //    await viewModel.Initialize();
    //  }
    //  finally
    //  {
    //    viewModel.IsInitializing = false;
    //  }

    //  var page = await ResolvePage(scope, viewModelType);
    //  if (page == null)
    //  {
    //    return null;
    //  }

    //  page.BindingContext = viewModel;
    //  page.SetBinding(Page.TitleProperty, "Title");

    //  return page;
    //}

    public async Task NavigateModalAsync<T>() where T : class, IViewModel
    {
      await InternalNavigateAsync<T>(async (p) =>
       {
         await Navigation.PushModalAsync(p);
       });
    }

    public async Task NavigateModalAsync<T, TParam>(TParam param) where T : class, IViewModel<TParam>
    {
      await InternalNavigateAsync<T>(async (p) => await Navigation.PushModalAsync(p), async (viewModel) =>
      {
        await viewModel.Initialize(param);
      });
    }

    public Task<TResult> NavigateModalAsync<T, TResult>(Func<Page, Task> navigation = null) where T : class, IViewModel
    {
      return InternalNavigateModalAsync<T, TResult>(navigation: navigation);
    }

    public async Task<TResult> InternalNavigateModalAsync<T, TResult>(Func<T, Task> init = null, Func<Page, Task> navigation = null) where T : class, IViewModel
    {
      if (navigation == null)
      {
        navigation = async (p) => await Navigation.PushModalAsync(p);
      }
      using (var scope = IoC.Container.BeginLifetimeScope())
      {
        try
        {
          var viewModel = scope.Resolve<T>();
          if (viewModel == null)
          {
            await PopToRootAsync();
            return default(TResult);
          }

          TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
          results.Add(viewModel, tcs);
          try
          {
            viewModel.IsInitializing = true;

            await viewModel.Initialize();
            if (init != null)
              await init(viewModel);
          }
          finally
          {
            viewModel.IsInitializing = false;
          }

          var page = await ResolvePage<T>(scope);
          if (page == null)
          {
            await PopToRootAsync();
            return default(TResult);
          }

          page.BindingContext = viewModel;

          page.SetBinding(Page.TitleProperty, "Title");

          //var page2 = new NavigationPage(page);
          //page2.Popped += Page_Popped;

          page.Disappearing += (s, args) =>
          {
            if (results.TryGetValue(viewModel, out TaskCompletionSource<object> taskToCancel))
            {
              taskToCancel.SetResult(default(TResult));
              Debug.WriteLine("Modal closed with hardware button");
            }
            scope.Dispose();
          };

          await navigation(page);

          return (TResult)await tcs.Task;
        }
        catch (Exception e)
        {
          Debug.WriteLine(e.Message);

          throw;
        }
      }
    }

    private void Page_Popped(object sender, NavigationEventArgs e)
    {
      if (e.Page.BindingContext is IDisposable disposableContect)
      {
        disposableContect.Dispose();
      }
    }

    public Task<TResult> NavigateModalAsync<T, TParam, TResult>(TParam param, Func<Page, Task> navigation = null) where T : class, IViewModel<TParam>
    {
      return this.InternalNavigateModalAsync<T, TResult>(async (viewModel) => await viewModel.Initialize(param), navigation);
    }

    public Task PopModalAsync<TViewModel>(TViewModel viewModel) where TViewModel : IViewModel
    {
      try
      {
        return Navigation.PopModalAsync();
      }
      finally
      {
        viewModel?.Dispose();
      }
    }

    public async Task PopModalAsync<T, TResult>(T viewModel, TResult result, Func<Task> navigation) where T : class, IViewModel
    {
      if (navigation == null)
      {
        navigation = () => Navigation.PopModalAsync();
      }

      try
      {
        if (!results.TryGetValue(viewModel, out TaskCompletionSource<object> tcs))
        {
          Debug.WriteLine("There is no viewmodel waiting for this result");
          await Navigation.PopModalAsync();
          return;
        }

        try
        {
          tcs.SetResult(result);

          results.Remove(viewModel);

          await navigation();
        }
        catch (Exception e)
        {
          tcs?.TrySetException(e);
        }
      }
      finally
      {
        viewModel?.Dispose();
      }
    }
  }
}
