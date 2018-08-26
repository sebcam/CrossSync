using System.Reflection;
using CrossSync.Entity.Abstractions;
using CrossSync.Entity.Abstractions.EF.UnitOfWork;
using CrossSync.Entity.Abstractions.Services;
using CrossSync.Entity.Abstractions.UnitOfWork;
using CrossSync.Entity.Server.Repositories;
using CrossSync.Infrastructure.Server;
using CrossSync.Infrastructure.Server.UnitOfWork;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CrossSync.AspNetCore.Extensions
{
  public static class ApplicationBuilderExtensions
  {
    public static IServiceCollection AddSync<TDbContext>(this IServiceCollection services) where TDbContext : ServerContext
    {
      //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
      //  .AddJsonOptions(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore)

      // Used to register Tombstone Controller
      //.AddApplicationPart(Assembly.GetExecutingAssembly()).AddControllersAsServices();

      services.AddMediatR(Assembly.GetExecutingAssembly(), Assembly.GetCallingAssembly());

      services.AddScoped<UnitOfWork<TDbContext>>();
      services.AddScoped<IUnitOfWork<TDbContext>>(s => s.GetService<UnitOfWork<TDbContext>>());
      services.AddScoped<IUnitOfWork>(s => s.GetService<UnitOfWork<TDbContext>>());

      services.AddScoped<IServerContext>(s => s.GetService<TDbContext>());
      services.AddScoped<IContext>(s => s.GetService<TDbContext>());

      services.AddScoped(typeof(ISyncRepository<>), typeof(SyncRepository<>));

      return services;
    }
  }
}
