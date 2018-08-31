using System.Reflection;
using Autofac;
using CrossSync.Entity.Abstractions.EF.UnitOfWork;
using CrossSync.Infrastructure.Client.UnitOfWork;
using CrossSync.Xamarin.DependencyInjection;
using CrossSync.Xamarin.Mvvm;
using CrossSync.Xamarin.Mvvm.Services.Connectivity;
using CrossSync.Xamarin.Mvvm.Services.Errors;
using CrossSync.Xamarin.Services;
using Microsoft.EntityFrameworkCore;
using Sample.TodoList.Entities.Shared;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Sample.TodoList.Xamarin
{
  public partial class App : CrossSyncApplication
  {
    private static string dbPath;

    public static void Init(string dbPath)
    {
      App.dbPath = dbPath;
    }

    public App()
    {
      InitializeComponent();

      MainPage = new NavigationPage(IoC.Resolve<MainPage>());
    }

    protected override void OnStart()
    {
      // Handle when your app starts
    }

    public override void ConfigureIoC(ContainerBuilder builder)
    {
      base.ConfigureIoC(builder);

      builder.Register<SyncConfiguration>(c => new SyncConfiguration { ApiBaseUrl = Constants.ApiBaseUrl, TombstoneUri = Constants.DeleteApiUri });

      builder.RegisterType<UnitOfWork<TodoListContext>>()
       .AsImplementedInterfaces()
       .As<IUnitOfWork<TodoListContext>>()
       .InstancePerLifetimeScope();

      builder.RegisterType<ConnectivityService>().AsImplementedInterfaces();
      builder.RegisterType<ErrorService>().AsImplementedInterfaces();

      builder.RegisterType<TodoListContext>().AsImplementedInterfaces().AsSelf()
        .WithParameter(new TypedParameter(typeof(string), dbPath))
        .OnActivated(f => f.Instance.Database.Migrate())
        .InstancePerLifetimeScope();

      var dataAccess = Assembly.GetExecutingAssembly();
      builder.RegisterAssemblyTypes(dataAccess)
             .Where(t => t.Name.EndsWith("Service")).AsImplementedInterfaces()
             .InstancePerLifetimeScope();
      // Register View Models
      builder.RegisterAssemblyTypes(dataAccess)
             .Where(t => t.Name.EndsWith("ViewModel")).AsSelf().InstancePerLifetimeScope();

      // Register Pages
      builder.RegisterAssemblyTypes(dataAccess)
             .Where(t => t.Name.EndsWith("Page")).Named<Page>(c => c.Name).AsSelf()
             .InstancePerDependency();
    }

    protected override void OnSleep()
    {
      // Handle when your app sleeps
    }

    protected override void OnResume()
    {
      // Handle when your app resumes
    }
  }
}
