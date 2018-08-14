using System.Collections.Generic;
using MediatR;

namespace CoreSync.Entity
{
  /// <summary>
  /// Notifiable interface
  /// </summary>
  public interface INotifiableEntity
  {
    /// <summary>
    /// Gets events to notify
    /// </summary>
    IEnumerable<INotification> DomainEvents { get; }
  }
}
