using System;

namespace CrossSync.Xamarin.Exceptions
{
  /// <summary>
  /// Represents error that occurs when synchronization server is not reachable
  /// </summary>
  public class ServerNotReachableException : Exception
  {
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="serverUrl"></param>
    public ServerNotReachableException(string serverUrl)
      : base($"Server not available : {serverUrl}.")
    {
    }
  }
}
