using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CoreSync.Xamarin.Services;
using Plugin.Connectivity.Abstractions;

namespace CoreSync.Xamarin.Mvvm.Services.Connectivity
{
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
}
