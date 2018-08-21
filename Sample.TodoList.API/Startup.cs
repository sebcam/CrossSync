using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreSync.AspNetCore;
using CoreSync.AspNetCore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sample.TodoList.Entities.Shared;
using Autofac;
using CoreSync.Entity.Abstractions.EF.UnitOfWork;
using CoreSync.Infrastructure.Server.UnitOfWork;
using CoreSync.Entity.Abstractions.UnitOfWork;
using CoreSync.Entity.Abstractions.Services;
using CoreSync.Entity.Server.Repositories;
using Sample.TodoList.API.Repositories;

namespace Sample.TodoList.API
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public IServiceProvider ConfigureServices(IServiceCollection services)
    {
      services.AddDbContext<TodoListContext>();

      services.Configure<ConnectionStringOptions>(Configuration);

      return services.AddSync(builder =>
      {
        builder.RegisterType<UnitOfWork<TodoListContext>>().AsImplementedInterfaces().InstancePerLifetimeScope();
        builder.RegisterType<TodoListRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseSync();
    }
  }
}
