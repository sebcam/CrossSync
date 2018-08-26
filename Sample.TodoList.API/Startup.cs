using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrossSync.AspNetCore;
using CrossSync.AspNetCore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sample.TodoList.Entities.Shared;
using CrossSync.Entity.Abstractions.EF.UnitOfWork;
using CrossSync.Infrastructure.Server.UnitOfWork;
using CrossSync.Entity.Abstractions.UnitOfWork;
using CrossSync.Entity.Abstractions.Services;
using CrossSync.Entity.Server.Repositories;
using Newtonsoft.Json;
using CrossSync.AspNetCore.Api;

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
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMvc().AddApplicationPart(typeof(TombstoneController).Assembly).AddControllersAsServices().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
        .AddJsonOptions(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

      services.Configure<ConnectionStringOptions>(Configuration);

      services.AddDbContext<TodoListContext>();

      services.AddSync<TodoListContext>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      app.UseMvc();

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      
    }
  }
}
