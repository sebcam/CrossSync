namespace CrossSync.Infrastructure.Client
{
  /// <summary>
  /// Client Operation status enumeration
  /// </summary>
  public enum OperationStatus
  {
    /// <summary>
    /// Entity is added
    /// </summary>
    Added = 1,
    /// <summary>
    /// Entity is updated
    /// </summary>
    Updated = 2,
    /// <summary>
    /// Entity is updated
    /// </summary>
    Deleted = 9
  }
}