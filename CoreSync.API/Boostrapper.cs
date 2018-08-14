using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace CoreSync.API
{
  public static class Boostrapper
  {
    public static IServiceProvider ConfigureServices(IServiceCollection services)
    {

      return InitializeIoC(services);
    }

    private static IServiceProvider InitializeIoC(IServiceCollection services)
    {
      var container = new ContainerBuilder();
      container.Populate(services);

      return new AutofacServiceProvider(container.Build());
    }
  }
}
