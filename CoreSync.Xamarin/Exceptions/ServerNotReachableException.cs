using System;

namespace CoreSync.Xamarin.Exceptions
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
      : base($"Le serveur n'est pas disponible : {serverUrl}.")
    {
    }
  }
}
