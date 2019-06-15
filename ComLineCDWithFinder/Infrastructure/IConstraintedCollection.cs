using System;
using System.Collections.Generic;

namespace ComLineCDWithFinder
{
    public interface IConstrainedCollection<T> : ICollection<T>
    {
        Predicate<T> Constraint { get; }
        bool TryAdd(T item);
        bool IsSizeLimited { get; }
        int? Size { get; }
    }
}