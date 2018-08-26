using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace CrossSync.Entity.Abstractions.Extensions
{
  public static class EntityEntryExtensions
  {
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="entry"></param>
    /// <param name="collection"></param>
    /// <param name="newValueCollection"></param>
    public static void SetChildrenCurrentValues<TEntity, T>(this EntityEntry<TEntity> entry, Func<TEntity, ICollection<T>> collection, IEnumerable<T> newValueCollection)
      where TEntity : Abstractions.Entity
      where T : Abstractions.Entity
    {
      var existing = entry.Entity;

      var realChildrenInstance = collection(existing);

      // ensure entity collection is loaded
      var entryCollection = entry.Collections.FirstOrDefault(f => f.CurrentValue == realChildrenInstance);
      if (entryCollection != null && !entryCollection.IsLoaded)
        entryCollection.Load();

      // delete chidren
      foreach (var existingChild in realChildrenInstance.ToList())
      {
        if (!newValueCollection.Any(c => c.Id == existingChild.Id))
        {
          entry.Context.Remove(existingChild);
          realChildrenInstance.Remove(existingChild);
        }
      }

      // Update and Insert children
      foreach (var childModel in newValueCollection)
      {
        var existingChild = realChildrenInstance
            .Where(c => c.Id == childModel.Id)
            .SingleOrDefault();

        if (existingChild != null)
        {
          var childEntry = entry.Context.Entry(existingChild);
          // Update child
          childEntry.CurrentValues.SetValues(childModel);

          //TODO : make recursive
        }
        else
        {
          collection(existing).Add(childModel);
        }
      }
    }

    //public static void SetChildrenCurrentValues<TEntity, T>(this EntityEntry<TEntity> entry, Func<TEntity, IReadOnlyCollection<T>> collection, IEnumerable<T> newValueCollection, Action<T> collectionRemove, Action<T> collectionAdd)
    // where TEntity : Abstractions.Entity
    // where T : Abstractions.Entity
    //{
    //  var existing = entry.Entity;

    //  var realChildrenInstance = collection(existing);

    //  foreach (var existingChild in realChildrenInstance.ToList())
    //  {
    //    if (!newValueCollection.Any(c => c.Id == existingChild.Id))
    //    {
    //      entry.Context.Remove(existingChild);
    //      collectionRemove(existingChild);
    //    }
    //  }

    //  // Update and Insert children
    //  foreach (var childModel in newValueCollection)
    //  {
    //    var existingChild = realChildrenInstance
    //        .Where(c => c.Id == childModel.Id)
    //        .SingleOrDefault();

    //    if (existingChild != null)
    //      // Update child
    //      entry.Context.Entry(existingChild).CurrentValues.SetValues(childModel);
    //    else
    //    {
    //      collectionAdd(childModel);
    //    }
    //  }
    //}

    /// <summary>
    /// Returns the typed Id property
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entry"></param>
    /// <returns></returns>
    public static T GetEntityId<T>(this EntityEntry entry) where T : struct
    {
      try
      {
        return (T)entry.Property("Id").CurrentValue;
      }
      catch
      {
        return default(T);
      }
    }

    /// <summary>
    /// Returns the entity version string value based on the "Version" property
    /// </summary>
    /// <param name="entry">The entity entry</param>
    /// <returns>The entity version string value</returns>
    public static string GetEntityVersion(this EntityEntry entry)
    {
      var versionProperty = entry.Property("Version");
      if (versionProperty == null || versionProperty.CurrentValue == null)
        return null;

      if (versionProperty.CurrentValue is byte[] timestampVersion)
      {
        return JsonConvert.SerializeObject(timestampVersion);
      }
      else
        return versionProperty.CurrentValue.ToString();
    }
  }
}
