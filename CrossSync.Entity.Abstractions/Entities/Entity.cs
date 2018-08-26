using System;

namespace CrossSync.Entity.Abstractions.Abstractions
{
  /// <summary>
  /// Entity base class
  /// </summary>
  public abstract class Entity : IIdentifiable
  {
    int? _requestedHashCode;
    Guid _Id;
    readonly Guid newId;

    /// <summary>
    /// Constructor
    /// </summary>
    public Entity()
    {
      newId = Guid.NewGuid();
      Id = newId;
    }

    /// <summary>
    /// Gets or sets the entity identifier
    /// </summary>
    public virtual Guid Id
    {
      get
      {
        return _Id;
      }
      set
      {
        _Id = value;
      }
    }

    /// <summary>
    /// Returns true if entity is newly added
    /// </summary>
    /// <returns></returns>
    public bool IsTransient()
    {
      return this.Id == newId;
    }

    /// <summary>
    /// Determines whether the object instance is the same
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
      if (obj == null || !(obj is Entity))
        return false;
      if (Object.ReferenceEquals(this, obj))
        return true;
      if (this.GetType() != obj.GetType())
        return false;
      Entity item = (Entity)obj;
      if (item.IsTransient() || this.IsTransient())
        return false;
      else
        return item.Id == this.Id;
    }

    /// <summary>
    /// Hash code
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
      if (!IsTransient())
      {
        if (!_requestedHashCode.HasValue)
          _requestedHashCode = this.Id.GetHashCode() ^ 31;
        // XOR for random distribution. See:
        // http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx
        return _requestedHashCode.Value;
      }
      else
        return base.GetHashCode();
    }

    /// <summary>
    /// Determines whether the object instances are the same
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(Entity left, Entity right)
    {
      if (Object.Equals(left, null))
        return (Object.Equals(right, null)) ? true : false;
      else
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether the object instances are not the same
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(Entity left, Entity right)
    {
      return !(left == right);
    }
  }
}
