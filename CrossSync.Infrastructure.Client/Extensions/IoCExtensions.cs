//using CrossSync.Infrastructure.Client;
//using Microsoft.EntityFrameworkCore;

//namespace Autofac
//{
//  /// <summary>
//  /// IoC Extensions class
//  /// </summary>
//  public static class IoCExtensions
//  {
//    /// <summary>
//    /// DbContext registration
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    /// <param name="builder"></param>
//    /// <param name="databasePath">The complete platform local db file path</param>
//    /// <returns></returns>
//    public static ContainerBuilder RegisterDbContext<T>(this ContainerBuilder builder, string databasePath)
//       where T : DbContext, IClientContext
//    {
//      builder.RegisterType<T>()
//        .AsImplementedInterfaces()
//        .AsSelf()
//        .WithParameter(new TypedParameter(typeof(string), databasePath))
//        .OnActivated(f => f.Instance.Database.Migrate())
//        .InstancePerLifetimeScope();
//      return builder;
//    }
//  }
//}
