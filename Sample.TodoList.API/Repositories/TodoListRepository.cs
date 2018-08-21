using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreSync.Entity.Abstractions;
using CoreSync.Entity.Server.Repositories;
using Sample.TodoList.Entities.Shared;

namespace Sample.TodoList.API.Repositories
{
  public class TodoListRepository : SyncRepository<TodoList.Entities.Shared.TodoList>
  {
    public TodoListRepository(TodoListContext context) : base(context)
    {
    }
  }
}
