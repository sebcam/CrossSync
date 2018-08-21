using Acr.UserDialogs;
using Autofac;
using CoreSync.Xamarin.DependencyInjection;
using CoreSync.Xamarin.Mvvm.Services.Connectivity;
using CoreSync.Xamarin.Mvvm.Services.Errors;
using CoreSync.Xamarin.Mvvm.Services.Navigation;
using CoreSync.Xamarin.Services;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using Xamarin.Forms;

namespace CoreSync.Xamarin.Mvvm
{
  /// <summary>
  /// Startup application 
  /// </summary>
  public abstract class CoreSyncApplication : Application
  {
    /// <summary>
    /// ctor
    /// </summary>
    public CoreSyncApplication()
    {
      InitIoC();
    }

    protected abstract string ApiBaseUrl {get; }
    protected abstract string DeletedRelativeUri { get; }

    private void InitIoC()
    {
      IoC.Configure(builder =>
      {
        builder.Register<IUserDialogs>((c) => UserDialogs.Instance);
        builder.Register<IConnectivity>(d => CrossConnectivity.Current);

        builder.RegisterType<ErrorService>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<ConnectivityService>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<NavigationService>().AsImplementedInterfaces().SingleInstance();

        builder.Register<string>(f => ApiBaseUrl).Named<string>("ApiBaseUrl");
        builder.Register<string>(f => DeletedRelativeUri).Named<string>("DeletedRelativeUri");
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
