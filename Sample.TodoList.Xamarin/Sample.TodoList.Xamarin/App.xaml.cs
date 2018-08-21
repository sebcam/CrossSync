using System;
using System.Reflection;
using Autofac;
using CoreSync.Entity.Abstractions.EF.UnitOfWork;
using CoreSync.Infrastructure.Client.UnitOfWork;
using CoreSync.Xamarin.DependencyInjection;
using CoreSync.Xamarin.Mvvm;
using Sample.TodoList.Entities.Shared;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Sample.TodoList.Xamarin
{
  public partial class App : CoreSyncApplication
  {
    private static string dbPath;

    public static void Init(string dbPath)
    {
      App.dbPath = dbPath;
    }

    protected override string ApiBaseUrl => Constants.ApiBaseUrl;

    protected override string DeletedRelativeUri => Constants.DeleteApiUri;

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

      builder.RegisterType<UnitOfWork<TodoListContext>>()
       .AsImplementedInterfaces()
       .As<IUnitOfWork<TodoListContext>>()
       .InstancePerLifetimeScope();

      builder.RegisterDbContext<TodoListContext>(dbPath);

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
