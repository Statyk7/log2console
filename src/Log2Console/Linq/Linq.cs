using System.Collections.Generic;

#if !HASLINQ

namespace System.Linq
{
  /// <summary>
  /// This class replaces standars LINQ to Objects libraries (to be able to run on vanilla .NET 2.0 Framework)
  /// Methods are added on need-to-use basis
  /// </summary>
  internal static class Enumerable
  {
    public static IEnumerable<TResult> Select<TInput, TResult>(this IEnumerable<TInput> source, Func<TInput, TResult> selector)
    {
      if (source == null)
        throw new ArgumentNullException("source");

      if (selector == null)
        throw new ArgumentNullException("selector");

      foreach (var i in source)
        yield return selector(i);
    }

    public static int Min(this IEnumerable<int> source)
    {
      if (source == null)
        throw new ArgumentNullException("source");

      var min = 0;
      var hasElements = false;

      foreach (var i in source)
      {
        if (hasElements)
        {
          if (i < min)
            min = i;
        }
        else
        {
          min = i;
          hasElements = true;
        }
      }

      if (!hasElements)
        throw new ArgumentException("source");

      return min;
    }

    public static int Max(this IEnumerable<int> source)
    {
      if (source == null)
        throw new ArgumentNullException("source");

      var max = 0;
      var hasElements = false;

      foreach (var i in source)
      {
        if (hasElements)
        {
          if (i > max)
            max = i;
        }
        else
        {
          max = i;
          hasElements = true;
        }
      }

      if (!hasElements)
        throw new ArgumentException("source");

      return max;
    }

    public static int Min<T>(this IEnumerable<T> source, Func<T, int> extractor)
    {
      return source.Select(extractor).Min();
    }

    public static int Max<T>(this IEnumerable<T> source, Func<T, int> extractor)
    {
      return source.Select(extractor).Max();
    }
  }
}

namespace System
{
  public delegate TResult Func<in TParam, out TResult>(TParam param);
}

namespace System.Runtime.CompilerServices
{
  public class ExtensionAttribute : Attribute { }
}

#endif
