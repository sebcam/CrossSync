using System;
using System.Collections.Generic;
using System.Text;
using CrossSync.Entity;

namespace Sample.TodoList.Entities.Shared
{
  public class TodoList : VersionableEntity
  {
    private List<TodoListItem> items;

    public TodoList()
    {
      Date = DateTime.UtcNow;
    }

    public string Name { get; set; }

    public DateTime Date { get; set; }

    public virtual IReadOnlyCollection<TodoListItem> Items => items ?? (items = new List<TodoListItem>());

    public bool Completed { get; set; }
  }
}
