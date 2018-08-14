using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace CoreSync.API.Extensions
{
  public static class ApplicationBuilderExtensions
  {
    public static IApplicationBuilder AddSync(this IApplicationBuilder appBuilder)
    {
      return appBuilder
        .UseMvc()
        .UseRequestLocalization();
    }

    public static IServiceProvider AddSync(this IServiceCollection services, Action<ContainerBuilder> iocOptions)
    {
      services
        .AddMvc()
        .AddJsonOptions(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

      services.AddMediatR(Assembly.GetExecutingAssembly(), Assembly.GetCallingAssembly());

      var container = new ContainerBuilder();
      container.Populate(services);

      iocOptions(container);

      ConfigureContainer(container);

      return new AutofacServiceProvider(container.Build());
    }


    private static void ConfigureContainer(ContainerBuilder builder)
    {

    }
  }
}
