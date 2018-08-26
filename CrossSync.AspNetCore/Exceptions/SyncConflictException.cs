//using System;
//using System.Collections.Generic;
//using System.Text;
//using CrossSync.Entity;

//namespace CrossSync.AspNetCore.Exceptions
//{
//  public class SyncConflictVersionException<T> : Exception where T : IVersionableEntity
//  {
//    public SyncConflictVersionException(T value, T existing) : base($"Version conflict occured.")
//    {
//      Value = value;
//      Existing = existing;
//    }

//    public T Value { get; }
//    public T Existing { get; }
//  }
//}
