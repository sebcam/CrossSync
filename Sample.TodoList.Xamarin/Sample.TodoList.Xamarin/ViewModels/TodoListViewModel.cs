using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CoreSync.Xamarin.Mvvm.Extensions;
using CoreSync.Xamarin.Mvvm.Models;
using CoreSync.Xamarin.Services;
using Xamarin.Forms;

namespace Sample.TodoList.Xamarin.ViewModels
{
  public class TodoListViewModel : BaseViewModel
  {
    private readonly IMobileSyncService<Entities.Shared.TodoList> service;
    private ICommand completedCommand;
    public TodoListViewModel(IMobileSyncService<Entities.Shared.TodoList> service)
    {
      this.service = service;
    }

    public ICommand CompletedCommand => completedCommand ?? (completedCommand = new Command<Guid>(async (d) =>
    {
      var selected = Todos.First(f => f.Id == d);
      selected.Completed = true;
      await service.UpdateAsync(d, selected);

      var todos = await service.GetAllAsync();
      Todos = todos.ToObservableCollection();
    }));

    public ObservableCollection<Entities.Shared.TodoList> Todos { get; set; }

    protected async override Task Initialize(CancellationToken cancellationToken)
    {
      await base.Initialize(cancellationToken);

      var todos = await service.GetAllAsync(f => !f.Completed);
      Todos = todos.ToObservableCollection();
    }
  }
}
