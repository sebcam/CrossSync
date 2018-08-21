using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sample.TodoList.Entities.Shared;

namespace Sample.TodoList.API
{
  public static class WebHostExtensions
  {

    public static IWebHost MigrateContext(this IWebHost webHost)
    {
      using (var scope = webHost.Services.CreateScope())
      {
        var context = scope.ServiceProvider.GetService<TodoListContext>();
        context.Database.Migrate();
      }

      return webHost;
    }
  }
}

