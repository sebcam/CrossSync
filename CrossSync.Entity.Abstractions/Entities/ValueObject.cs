using System.Collections.Generic;

namespace CrossSync.Entity.Abstractions.Abstractions
{
  /// <summary>
  /// Value object base class used for complex members
  /// </summary>
  public abstract class ValueObject
  {
    /// <summary>
    /// Returns the atomic values used for comparison
    /// </summary>
    /// <returns></returns>
    protected abstract IEnumerable<object> GetAtomicValues();

    /// <summary>
    /// Compares object values
    /// </summary>
    /// <param name="object"></param>
    /// <returns></returns>
    public bool ValuesEqual(ValueObject @object)
    {
      IEnumerator<object> thisValues = GetAtomicValues().GetEnumerator();
      IEnumerator<object> otherValues = @object.GetAtomicValues().GetEnumerator();
      while (thisValues.MoveNext() && otherValues.MoveNext())
      {
        if (thisValues.Current is null ^ otherValues.Current is null)
        {
          return false;
        }
        if (thisValues.Current != null && !thisValues.Current.Equals(otherValues.Current))
        {
          return false;
        }
      }
      return !thisValues.MoveNext() && !otherValues.MoveNext();
    }

    /// <summary>
    /// Copy the current object
    /// </summary>
    /// <returns></returns>
    public ValueObject Copy()
    {
      return this.MemberwiseClone() as ValueObject;
    }
  }
}
