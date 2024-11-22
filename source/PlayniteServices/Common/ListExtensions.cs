using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Playnite;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

/// <summary>
/// Represents class with various extension methods for IEnumerable lists.
/// </summary>
public static class ListExtensions
{
    /// <summary>
    /// Converts collection to <see cref="ObservableCollection{T}"/> collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
    {
        return new ObservableCollection<T>(source);
    }

    /// <summary>
    /// Check if collection has any items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool HasItems<T>([NotNullWhen(true)] this IEnumerable<T>? source)
    {
        if (source is null)
        {
            return false;
        }

        if (source is IReadOnlyCollection<T> col)
        {
            return col.Count > 0;
        }

        if (source is IList<T> list)
        {
            return list.Count > 0;
        }

        if (source is T[] array)
        {
            return array.Length > 0;
        }

        return source.Any();
    }

    /// <summary>
    /// Check if collection has any items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static bool HasItems<T>([NotNullWhen(true)] this IEnumerable<T>? source, Func<T, bool> predicate)
    {
        return source?.Any(predicate) == true;
    }

    /// <summary>
    /// Adds new item to the list only if it's not already part of the list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="item"></param>
    /// <returns>True if item was added, false if it's already part of the list.</returns>
    public static bool AddMissing<T>(this IList<T> source, T item)
    {
        if (!source.Contains(item))
        {
            source.Add(item);
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool AddMissingBy<T>(this IList<T> source, T item, Func<T, bool> predicate)
    {
        if (!source.Any(predicate))
        {
            source.Add(item);
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Adds new items to the list only if they are not already part of the list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="items"></param>
    /// <returns>True if an item was added, false if no item was added.</returns>
    public static bool AddMissing<T>(this IList<T> source, IEnumerable<T> items)
    {
        var anyAdded = false;
        foreach (var item in items)
        {
            if (AddMissing(source, item))
            {
                anyAdded = true;
            }
        }

        return anyAdded;
    }

    /// <summary>
    /// Checks if collection has any non-empty string items.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool HasNonEmptyItems([NotNullWhen(true)] this IEnumerable<string>? source)
    {
        return source?.Any(a => !string.IsNullOrEmpty(a)) == true;
    }

    /// <summary>
    /// Checks if source collection contains any items from target one, even if just partially.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="comparison"></param>
    /// <returns>True if target collection contains items that are also part of source collection.</returns>
    public static bool IntersectsPartiallyWith([NotNullWhen(true)] this IEnumerable<string>? source, [NotNullWhen(true)] IEnumerable<string>? target, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
    {
        if (source != null && target != null)
        {
            foreach (var sourceItem in source)
            {
                if (target.Any(a => a?.IndexOf(sourceItem, comparison) >= 0))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if source collection contains any items from target one.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="comparison"></param>
    /// <returns>True if target collection contains items that are also part of source collection.</returns>
    public static bool IntersectsExactlyWith([NotNullWhen(true)] this IEnumerable<string>? source, [NotNullWhen(true)] IEnumerable<string>? target, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
    {
        if (source != null && target != null)
        {
            foreach (var sourceItem in source)
            {
                if (target.Any(a => a?.Equals(sourceItem, comparison) == true))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if source collection contains specified string completely.
    /// </summary>
    public static bool ContainsString([NotNullWhen(true)] this IEnumerable<string>? source, string value, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
    {
        return source?.Any(a => a.Equals(value, comparison)) == true;
    }

    /// <summary>
    /// Checks if part of specified string is part of the collection.
    /// </summary>
    public static bool ContainsStringPartial([NotNullWhen(true)] this IEnumerable<string>? source, string value, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
    {
        return source?.Any(a => a.Contains(value, comparison)) == true;
    }

    /// <summary>
    /// Checks if source collection contains part of specified string.
    /// </summary>
    public static bool ContainsPartOfString([NotNullWhen(true)] this IEnumerable<string>? source, string value, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
    {
        return source?.Any(a => value.Contains(a, comparison)) == true;
    }

    /// <summary>
    /// Checks if two collections contain the same items in any order.
    /// </summary>
    public static bool IsListEqual<T>(this IList<T>? source, IList<T>? target)
    {
        if (source is null && target is null)
        {
            return true;
        }

        if (source is not null && target is not null)
        {
            if (source.Count != target.Count)
            {
                return false;
            }

            if (source.Except(target).Any())
            {
                return false;
            }

            return true;
        }

        return false;
    }

    public static bool IsHashSetEqual<T>(this HashSet<T>? source, HashSet<T>? target)
    {
        if (source is null && target is null)
        {
            return true;
        }

        if (target is null)
        {
            return false;
        }

        return source?.SetEquals(target) == true;
    }

    public static bool IsDictionaryEqual<TKey, TValue>(this Dictionary<TKey, TValue>? source, Dictionary<TKey, TValue>? target) where TKey : notnull
    {
        if (source is null && target is null)
        {
            return true;
        }

        if (source is not null && target is not null)
        {
            if (source.Count != target.Count)
            {
                return false;
            }

            return source.Keys.All(a => target.ContainsKey(a) && EqualityComparer<TValue>.Default.Equals(source[a], target[a]));
        }

        return false;
    }

    /// <summary>
    /// Checks if two collections contain the same items in any order.
    /// </summary>
    public static bool IsListEqual<T>(this IList<T>? source, IList<T>? target, IEqualityComparer<T> comparer)
    {
        if (source is null && target is null)
        {
            return true;
        }

        if (source is not null && target is not null)
        {
            if (source.Count != target.Count)
            {
                return false;
            }

            if (source.Except(target, comparer).Any())
            {
                return false;
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if two collections contain the same items in the same order.
    /// </summary>
    public static bool IsListEqualExact<T>(this IList<T>? source, IList<T>? target)
    {
        if (source is null && target is null)
        {
            return true;
        }

        if (source is not null && target is not null)
        {
            if (source.Count != target.Count)
            {
                return false;
            }

            return source.SequenceEqual(target);
        }

        return false;
    }

    /// <summary>
    /// Check if collection contains all items from other collection (in any order).
    /// </summary>
    public static bool Contains<T>([NotNullWhen(true)] this IList<T>? source, [NotNullWhen(true)] IList<T>? target)
    {
        if (source != null && target != null)
        {
            if (target.Count > source.Count)
            {
                return false;
            }

            return target.Intersect(source).Count() == target.Count;
        }

        return false;
    }

    /// <summary>
    /// Gets items contained in all colletions.
    /// </summary>
    public static HashSet<T> GetCommonItems<T>(IEnumerable<IEnumerable<T>?> lists)
    {
        if (lists?.Any() != true || lists?.First().HasItems() != true)
        {
            return [];
        }

        var set = new HashSet<T>(lists.First()!);
        foreach (var list in lists)
        {
            if (list == null)
            {
                set.IntersectWith([]);
            }
            else
            {
                set.IntersectWith(list);
            }
        }

        return set;
    }

    /// <summary>
    /// Gets items distinct to all collections.
    /// </summary>
    public static HashSet<T> GetDistinctItems<T>(IEnumerable<IEnumerable<T>?> lists)
    {
        if (lists.HasItems() != true)
        {
            return [];
        }

        var set = new List<T>();
        foreach (var list in lists)
        {
            if (list != null)
            {
                set.AddRange(list);
            }
        }

        var listsCounts = lists.Count();
        return new HashSet<T>(set.GroupBy(a => a).Where(a => a.Count() < listsCounts).Select(a => a.Key));
    }

    /// <summary>
    /// Gets items distinct to all collections.
    /// </summary>
    public static HashSet<T> GetDistinctItemsP<T>(params IEnumerable<T>[] lists)
    {
        if (lists.Length == 0)
        {
            return [];
        }

        return GetDistinctItems(lists.ToList());
    }

    public static bool AddRange<T>(this HashSet<T> source, IEnumerable<T>? toAdd)
    {
        if (toAdd is null)
        {
            return false;
        }

        var anyAdded = false;
        foreach (var item in toAdd)
        {
            if (source.Add(item))
            {
                anyAdded = true;
            }
        }

        return anyAdded;
    }

    public static HashSet<T> Merge<T>(params HashSet<T>?[] lists)
    {
        var allItems = new HashSet<T>();
        foreach (var list in lists)
        {
            allItems.AddRange(list);
        }

        return allItems;
    }

    /// <summary>
    /// Merge collections together.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="lists"></param>
    /// <returns></returns>
    public static List<T> Merge<T>(IEnumerable<IEnumerable<T>> lists)
    {
        var allItems = new List<T>();
        foreach (var list in lists)
        {
            allItems.AddRange(list);
        }

        return allItems;
    }

    /// <summary>
    /// Merge two collections together.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list1"></param>
    /// <param name="list2"></param>
    /// <returns></returns>
    public static List<T> Merge<T>(IList<T> list1, IList<T>? list2)
    {
        if (list1.HasItems() && list2.HasItems())
        {
            var allItems = new List<T>(list1.Count + list2.Count);
            allItems.AddRange(list1);
            allItems.AddRange(list2);
            return allItems;
        }
        else if (list1.HasItems() && !list2.HasItems())
        {
            return list1.ToList();
        }
        else if (!list1.HasItems() && list2.HasItems())
        {
            return list2.ToList();
        }

        return [];
    }

    public static bool AddIfNotNull<T>(this IList<T> source, [NotNullWhen(true)] T? item)
    {
        if (item == null)
        {
            return false;
        }

        source.Add(item);
        return true;
    }

    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? source)
    {
        return source ?? Enumerable.Empty<T>();
    }

    public static void ForEach<T>(this IEnumerable<T>? source, Action<T> action)
    {
        if (source is null)
        {
            return;
        }

        foreach (var item in source)
        {
            action(item);
        }
    }

    public static void MoveItemUp<T>(this IList<T> source, T item)
    {
        var oldIndex = source.IndexOf(item);
        if (oldIndex <= 0)
        {
            return;
        }

        if (source is ObservableCollection<T> obCol)
        {
            obCol.Move(oldIndex, oldIndex - 1);
        }
        else
        {
            source.RemoveAt(oldIndex);
            source.Insert(oldIndex - 1, item);
        }
    }

    public static void MoveItemDown<T>(this IList<T> source, T item)
    {
        var oldIndex = source.IndexOf(item);
        if (oldIndex == -1 || oldIndex == source.Count - 1)
        {
            return;
        }

        if (source is ObservableCollection<T> obCol)
        {
            obCol.Move(oldIndex, oldIndex + 1);
        }
        else
        {
            source.RemoveAt(oldIndex);
            source.Insert(oldIndex + 1, item);
        }
    }
}
