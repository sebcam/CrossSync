using CrossSync.AspNetCore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Sample.TodoList.Entities.Shared;
using Swashbuckle.AspNetCore.Swagger;

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
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
        .AddJsonOptions(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new Info { Title = "Sample todolist API", Version = "v1" });
      });


      services.Configure<ConnectionStringOptions>(Configuration);

      services.AddDbContext<TodoListContext>()
        .AddSync<TodoListContext>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      app.UseMvc();

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      app.UseSwagger();
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sample todolist API");
      });

    }
  }
}
