using System;
using Autofac;

namespace CoreSync.Xamarin.DependencyInjection
{
  /// <summary>
  /// IoC wrapper
  /// </summary>
  public static class IoC
  {
    /// <summary>
    /// Initialize the container
    /// </summary>
    /// <param name="builderAction"></param>
    public static void Configure(Action<ContainerBuilder> builderAction)
    {
      var builder = new ContainerBuilder();
      builderAction(builder);
      Container = builder.Build();
    }

    /// <summary>
    /// Gets the IoC container
    /// </summary>
    public static IContainer Container { get; private set; }

    /// <summary>
    /// Returns the resolved type
    /// </summary>
    /// <typeparam name="T">Type of object to resolve</typeparam>
    /// <returns></returns>
    public static T Resolve<T>()
    {
      return Container.Resolve<T>();
    }

    /// <summary>
    /// Returns the resolved type by name
    /// </summary>
    /// <typeparam name="T">Type of object to resolve</typeparam>
    /// <param name="name">Name of object to resolve</param>
    /// <returns></returns>
    public static T ResolveNamed<T>(string name)
    {
      return Container.ResolveNamed<T>(name);
    }

    /// <summary>
    /// Returns the resolved type
    /// </summary>
    /// <typeparam name="T">Type to be resolved</typeparam>
    /// <param name="type"></param>
    /// <returns></returns>
    public static T Resolve<T>(Type type)
    {
      return (T)ResolveType(type);
    }

    /// <summary>
    /// Resolve object by type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static object ResolveType(Type type)
    {
      return Container.Resolve(type);
    }
  }
}
