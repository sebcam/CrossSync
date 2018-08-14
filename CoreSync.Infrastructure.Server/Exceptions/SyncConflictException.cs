using System;
using CoreSync.Entity;

namespace CoreSync.Entity.Server.Exceptions
{
  public class SyncConflictVersionException<T> : Exception where T : IVersionableEntity
  {
    public SyncConflictVersionException(T value, T existing) : base($"Version conflict occured.")
    {
      Value = value;
      Existing = existing;
    }

    public T Value { get; }
    public T Existing { get; }
  }
}
