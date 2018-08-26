//using System;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace CrossSync.Infrastructure.Server
//{
//  /// <summary>
//  /// Represents an deleted entity
//  /// </summary>
//  public class DeletedEntity
//  {
//    /// <summary>
//    /// Unique identifier
//    /// </summary>
//    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//    public Guid Id { get; set; }

//    /// <summary>
//    /// Deleted entity identifier
//    /// </summary>
//    public Guid EntityId { get; set; }

//    /// <summary>
//    /// Entity data type
//    /// </summary>
//    public string DataType { get; set; }

//    /// <summary>
//    /// Delete date
//    /// </summary>
//    public DateTimeOffset DeletedDate { get; set; }

//  }
//}