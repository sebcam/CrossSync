namespace CrossSync.Entity
{
  /// <summary>
  /// Versionable entity client side implementation
  /// </summary>
  public class VersionableEntity : CrossSync.Entity.Abstractions.Abstractions.Entity, IVersionableEntity
  {
    /// <summary>
    /// Gets the entity version.
    /// This property should not be modified by clients application
    /// </summary>
    public string Version { get; set; }
  }
}
