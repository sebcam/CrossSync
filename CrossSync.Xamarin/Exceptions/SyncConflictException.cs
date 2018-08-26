using System;
using Newtonsoft.Json.Linq;

namespace CrossSync.Xamarin.Exceptions
{
  /// <summary>
  /// Represents errors that occur when a synchronization conflict is detected
  /// </summary>
  public class SyncConflictVersionException : Exception
  {
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="clientValue"></param>
    /// <param name="serverValue"></param>
    public SyncConflictVersionException(JObject clientValue, JObject serverValue) : base("Version Conflict occured.")
    {
      ClientValue = clientValue;
      ServerValue = serverValue;
    }

    public JObject ServerValue { get; }
    public JObject ClientValue { get; }
  }
}
