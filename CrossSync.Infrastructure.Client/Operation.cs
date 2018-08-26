using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrossSync.Infrastructure.Client
{
  /// <summary>
  /// Client Operation entity
  /// </summary>
  public class Operation
  {
    /// <summary>
    /// Identifier
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// Modified entity identifier
    /// </summary>
    public Guid EntityId { get; set; }
    
    /// <summary>
    /// Entity Type. Default use is Entity class name
    /// </summary>
    public string DataType { get; set; }

    /// <summary>
    /// Entity version
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// Entity update date
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Operation Status
    /// </summary>
    public EntityState Status { get; set; }

    public override string ToString()
    {
      return $"Operation : {Status} on entities {DataType} with Id '{EntityId}'";
    }
  }
}