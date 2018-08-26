using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossSync.Xamarin.Mvvm.Services.Navigation;
using CrossSync.Xamarin.Services;
using Sample.TodoList.Xamarin.ViewModels;
using Xamarin.Forms;

namespace Sample.TodoList.Xamarin
{
  public partial class MainPage : ContentPage
  {
    private readonly INavigationService navigationService;
    private readonly Lazy<SynchronizationService> syncService;

    public MainPage(INavigationService navigationService, Lazy<SynchronizationService> syncService)
    {
      InitializeComponent();
      this.navigationService = navigationService;
      this.syncService = syncService;
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
      await navigationService.NavigateAsync<TodoListViewModel>();
    }

    private async void Button_Clicked_1(object sender, EventArgs e)
    {
      await syncService.Value.SyncAsync();
    }
  }
}
