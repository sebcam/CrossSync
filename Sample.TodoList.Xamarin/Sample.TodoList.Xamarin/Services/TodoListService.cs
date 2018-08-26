using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using CrossSync.Entity.Abstractions;
using CrossSync.Entity.Abstractions.EF.UnitOfWork;
using CrossSync.Xamarin.DependencyInjection;
using CrossSync.Xamarin.Services;
using Sample.TodoList.Entities.Shared;

namespace Sample.TodoList.Xamarin.Services
{
  public class TodoListService : SyncService<Entities.Shared.TodoList>, IConflictHandler<TodoList.Entities.Shared.TodoList>
  {
    private readonly IUnitOfWork<TodoListContext> unitOfWork;

    public TodoListService(IUnitOfWork<TodoListContext> unitOfWork, IConnectivityService connectivityService, Lazy<IErrorService> errorService, SyncConfiguration config) : base(unitOfWork, connectivityService, errorService, config)
    {
      this.unitOfWork = unitOfWork;
    }

    public override string ApiUri => "api/todolist";

    public override int Order => 100;

    public async Task<Entities.Shared.TodoList> HandleConflict(Entities.Shared.TodoList clientValue, Entities.Shared.TodoList serverValue)
    {
      var selectedResult = await IoC.Resolve<IUserDialogs>().ActionSheetAsync("Conflict resolution", "Keep local version", null, buttons: new[] { "Take Server version" });
      return selectedResult == "Keep local version" ? clientValue : serverValue;
    }
  }
}
