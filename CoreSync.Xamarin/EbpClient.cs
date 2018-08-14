using CoreSync.Xamarin.Services;

namespace Autofac
{
  /// <summary>
  /// IoC Extensions
  /// </summary>
  public static class IoCExtensions
  {
    /// <summary>
    /// Register sync services
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="baseUrl"></param>
    /// <param name="deletedUri"></param>
    /// <returns></returns>
    public static ContainerBuilder RegisterSync(this ContainerBuilder builder, string baseUrl, string deletedUri)
    {
      builder.Register<string>(f => baseUrl).Named<string>("ApiBaseUrl");
      builder.Register<string>(f => deletedUri).Named<string>("DeletedRelativeUri");
      builder.RegisterType<SynchronizationService>().AsSelf().InstancePerLifetimeScope();
      return builder;
    }
  }
}
