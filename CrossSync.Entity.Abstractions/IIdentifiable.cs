using System;

namespace CrossSync.Entity.Abstractions
{
  public interface IIdentifiable
  {
    /// <summary>
    /// Gets the object identifier
    /// </summary>
    //TODO : Make type generic but probleme for generating Id on new object in offline context. NewGuid make it easier.
    Guid Id { get; }
  }
}
