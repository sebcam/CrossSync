using CoreSync.Entity.Abstractions;

namespace CoreSync.Entity
{
  /// <summary>
  /// Versionable entity client side interface
  /// </summary>
  public interface IVersionableEntity : IIdentifiable
  {
    /// <summary>
    /// Gets the entity version.
    /// This property should not be modified by clients application
    /// </summary>
    string Version { get; set; }
  }
}
