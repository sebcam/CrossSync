using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreSync.Xamarin.Services
{
  public interface IConnectivityService
  {
    bool IsConnected { get;  }

    Task<bool> IsRemoteReachable(string url);
  }
}
