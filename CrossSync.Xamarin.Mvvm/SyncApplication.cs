using Acr.UserDialogs;
using Autofac;
using CrossSync.Xamarin.DependencyInjection;
using CrossSync.Xamarin.Mvvm.Services.Connectivity;
using CrossSync.Xamarin.Mvvm.Services.Errors;
using CrossSync.Xamarin.Mvvm.Services.Navigation;
using CrossSync.Xamarin.Services;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using Xamarin.Forms;

namespace CrossSync.Xamarin.Mvvm
{
  /// <summary>
  /// Startup application 
  /// </summary>
  public abstract class CrossSyncApplication : Application
  {
    /// <summary>
    /// ctor
    /// </summary>
    public CrossSyncApplication()
    {
      InitIoC();
    }

    private void InitIoC()
    {
      IoC.Configure(builder =>
      {
        builder.Register<IUserDialogs>((c) => UserDialogs.Instance);
        builder.Register<IConnectivity>(d => CrossConnectivity.Current);

        builder.RegisterType<ErrorService>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<ConnectivityService>().AsImplementedInterfaces().SingleInstance();


        builder.RegisterType<NavigationService>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<SynchronizationService>().AsSelf().InstancePerLifetimeScope();

        ConfigureIoC(builder);
      });      
    }

    /// <summary>
    /// Configures the IoC container
    /// </summary>
    /// <param name="builder"></param>
    public virtual void ConfigureIoC(ContainerBuilder builder)
    {
    }
  }
}
