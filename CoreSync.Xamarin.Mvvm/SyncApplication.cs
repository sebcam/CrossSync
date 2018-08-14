using Acr.UserDialogs;
using Autofac;
using CoreSync.Xamarin.Dependency;
using CoreSync.Xamarin.Mvvm.Services.Navigation;
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

    private void InitIoC()
    {
      IoC.Configure(builder =>
      {
        builder.Register<IUserDialogs>((c) => UserDialogs.Instance);
        builder.Register<IConnectivity>(d => CrossConnectivity.Current);

        builder.RegisterType<NavigationService>().AsImplementedInterfaces().SingleInstance();
        
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
