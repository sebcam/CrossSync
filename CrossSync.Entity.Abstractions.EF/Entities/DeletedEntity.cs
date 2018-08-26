using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrossSync.Entity.Abstractions.Entities
{
  /// <summary>
  /// Class used to identify deleted entities
  /// </summary>
  public class DeletedEntity
  {
    /// <summary>
    /// Unique Identifier
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// Entity Identifier
    /// </summary>
    public Guid EntityId { get; set; }

    /// <summary>
    /// Entity Type. Default use is Entity class name
    /// </summary>
    public string DataType { get; set; }

    /// <summary>
    /// Gets or sets the deleted date
    /// </summary>
    public DateTimeOffset DeletedDate { get; set; }

  }
}