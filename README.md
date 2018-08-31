# CrossSync
![.NetCore](https://img.shields.io/badge/.NetCore-2.1-blue.svg) ![.NetStandard](https://img.shields.io/badge/.NetStandard-2.0-blue.svg)

CrossSync provides cross-plaform librairies for Xamarin and AspNetCore for easy data synchronization with offline data support based on Entity Framework Core.

### Why ?

Entity Framework Core now offers a way to share the business data code between the mobile and server applications.

For now, Azure Mobile App is the most popular framework to do this. But it's working with .NetFramework and bring it to .NetCore seems to be not announced yet.
So to work with Azure Mobile App, your data layer cannot be shared across these different platforms.

This libs can be usefull for developpers which are looking for a way to synchronize offline datas on your mobile application using Entity Framework Core.

High level features are :

 - EntityFramework use for both mobile (SQLite) and API (SQL Server)
 - Offline data support
 - Repository & Unit of work patterns
 - ...

## Top level Packages
| Name | Description | Nuget |
| ---- | ----------- | ----- |
| `CrossSync.AspNetCore` | Provides aspnet core sync tools | ![Version](https://img.shields.io/badge/-0.1.0-blue.svg) |
| `CrossSync.Xamarin`| Provides all implementations for mobile data offline and sync | ![Version](https://img.shields.io/badge/-0.1.0-blue.svg) |

## Get Started

### Server side

On your AspNetCore API Project :

    nuget install CrossSync.AspNetCore

##### Entity framework configuration
If you are not aware with EF, see [EF Core docs](https://docs.microsoft.com/en-us/ef/core/)

Your **DbContext** have to inherit from **ServerContext** class. 

All of your entities which need to be synchronized with mobile have to extend **VersionableEntity** class.

 1. Your entities have to inherit from **VersionableEntity** class or implement **IVersionableEntity**.
 2. Your EF Context have to inherit from **ServerContext** class.

##### Controllers configuration

Controllers have to inherit from **SyncController** which is a crud controller.

When you are done, juste add sync registration into your startup.cs :

    public void ConfigureServices(IServiceCollection services)
    {
	  /* ... */
	  
	  // Sync services registration
	  services.AddSync();
    }
    

### Client Side using Xamarin.Forms

#### Installation
On your Core assembly : 

    nuget install CrossSync.Xamarin

#### Configuration

This library needs to know about the connectivity status.

Create a class which implements IConnectivityService.
For example, the following uses the [Plugin Connectivity](https://github.com/jamesmontemagno/ConnectivityPlugin) from James Montemagno.

    public class ConnectivityService : IConnectivityService
    {
        private readonly IConnectivity connectivity;

        public ConnectivityService(IConnectivity connectivity)
        {
            this.connectivity = connectivity;
        }

        public bool IsConnected => connectivity.IsConnected;

        public Task<bool> IsRemoteReachable(string url)
        {
            return connectivity.IsRemoteReachable(url);
        }
    }

It also need a service to show errors during synchornization by implementing IErrorService.
For example, the following uses [Acr UserDialogs](https://github.com/aritchie/userdialogs) :


    public class ErrorService : IErrorService
    {
        private readonly IUserDialogs dialogs;

        public ErrorService(IUserDialogs dialogs)
        {
            this.dialogs = dialogs;
        }
        public void ShowError(string error)
        {
            using (dialogs.Toast(new ToastConfig(error) { Duration = new TimeSpan(0, 0, 2), Position = ToastPosition.Top })) { }
        }
    }

The last step is to register services to the IoC container :

    // using autofac
    builder.RegisterType<ErrorService>().AsImplementedInterfaces().SingleInstance();
    builder.RegisterType<ConnectivityService>().AsImplementedInterfaces().SingleInstance();
    builder.RegisterType<SynchronizationService>().AsSelf().InstancePerLifetimeScope();


#### How to use

##### Business Data Services implementation 

    public class TodoListService : SyncService<Entities.Shared.TodoList>
    {
        private readonly IUnitOfWork<TodoListContext> unitOfWork;

        public TodoListService(IUnitOfWork<TodoListContext> unitOfWork, IConnectivityService connectivityService, Lazy<IErrorService> errorService, SyncConfiguration config) : base(unitOfWork, connectivityService, errorService, config)
        {
            this.unitOfWork = unitOfWork;
        }

        public override string ApiUri => "api/todolist";

        // Order in which sync is done between services depending on data relations
        public override int Order => 100;
    }

##### Start a synchronization

Synchronization, for now, have to be manually launched like this :   

    // Resolve the registrated sync service
    var synchronizationService = IoC.Resolve<SynchronizationService>();
    // Starts the synchronization
    await synchronizationService.SyncAsync();

Soon, the synchronization will be done within an async background task.

##### Handle conflicts

In order to handler version conflicts between offline and server datas, you need to implement IConflictHandler<> on your data service

    public class TodoListService : SyncService<Entities.TodoList>, IConflictHandler<Entities.Shared.TodoList>
    {
        /* ... */

        public async Task<Entities.Shared.TodoList> HandleConflict(Entities.Shared.TodoList clientValue, Entities.Shared.TodoList serverValue)
        {
            // Ask for user to choose between client and server version
            var selectedResult = await IoC.Resolve<IUserDialogs>().ActionSheetAsync("Conflict resolution", "Keep local version", null, buttons: new[] { "Take Server version" });
            return selectedResult == "Keep local version" ? clientValue : serverValue;
        }
    }
